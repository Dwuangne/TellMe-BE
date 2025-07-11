﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TellMe.API.Constants;
using TellMe.Repository.Infrastructures;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using TellMe.Repository.Enities;
using TellMe.Repository.DBContexts;
using Microsoft.EntityFrameworkCore;
using TellMe.Service.Services.Interface;
using TellMe.Service.Services;
using Microsoft.Extensions.FileProviders;
using TellMe.Repository.SMTPs.Repositories;
using TellMe.Repository.Redis.Repositories;
using Redis.OM;
using TellMe.Service.Mapping;

namespace TellMe.API.Extensions
{
    public static class DependencyExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình Database Contexts
            services.AddDbContext<TellMeDBContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("TellMeConnectionString"),
                    b => b.MigrationsAssembly("TellMe.Repository")));

            services.AddDbContext<TellMeAuthDBContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("TellMeAuthConnectionString"),
                    b => b.MigrationsAssembly("TellMe.Repository")));

            // Cấu hình Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("TellMe")
                .AddEntityFrameworkStores<TellMeAuthDBContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtAuth:Issuer"],
                    ValidAudience = configuration["JwtAuth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuth:Key"]))
                };

                // Cấu hình cho SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Lấy token từ query string cho SignalR
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Cấu hình CORS cho SignalR
            services.AddCors(cors => cors.AddPolicy(
                name: CorsConstant.PolicyName,
                policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "https://fe.tellme.io.vn"
                    )
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); // Quan trọng cho SignalR
                }));

            // Cấu hình SignalR
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true; // Chỉ bật trong development
                options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            });

            // Dependency Injection
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IPsychologicalTestService, PsychologicalTestService>();
            services.AddScoped<IUserTestService, UserTestService>();
            services.AddScoped<IPsychologicalService, PsychologicalService>();
            services.AddScoped<IPsychologistReviewService, PsychologistReviewService>();
            services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IPayOsService, PayOsService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IUserPromotionService, UserPromotionService>();
            services.AddScoped<IPatientProfileService, PatientProfileService>();
            services.AddScoped<IPsychologicalAssessmentService, PsychologicalAssessmentService>();

            // Chat Services
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IParticipantService, ParticipantService>();

            services.AddSingleton<IEmailRepository, EmailRepository>();
            services.AddSingleton<IAccountTokenRedisRepository, AccountTokenRedisRepository>();

            // Cấu hình AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfiles));
        }
    }
}
