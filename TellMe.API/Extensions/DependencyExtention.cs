using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TellMe.API.Constants;
using TellMe.API.Middlewares;
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

namespace TellMe.API.Extensions
{
    public static class DependencyExtention
    {
        public static void AddDbContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<TellMeDBContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("TellMeConnectionString"),
                    b => b.MigrationsAssembly("TellMe.Repository")));
            builder.Services.AddDbContext<TellMeAuthDBContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("TellMeAuthConnectionString"),
                    b => b.MigrationsAssembly("TellMe.Repository")));
        }
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            //services.AddAutoMapper(typeof(AutoMapperProfiles));
            return services;
        }
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
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
            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRedisService, RedisService>();
            return services;
        }
            
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IEmailRepository, EmailRepository>();
            services.AddSingleton<IAccountTokenRedisRepository, AccountTokenRedisRepository>();
            return services;    
        }

        public static void AddJwtValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWTAuth:Issuer"],
                    ValidAudience = builder.Configuration["JWTAuth:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTAuth:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tell Me Application API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer (Token here)\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "Oauth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            return services;
        }

        public static WebApplication AddApplicationConfig(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates")),
                RequestPath = "/Templates"
            });
            app.UseCors(CorsConstant.PolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => "Welcome to Tell Me Application API!");
            return app;
        }
    }
}
