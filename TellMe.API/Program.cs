using DotNetEnv;
using Microsoft.Extensions.Logging;
using TellMe.API.Constants;

using TellMe.API.Extensions;
using TellMe.Repository.SMTPs.Models;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Log to console
    logging.AddDebug();   // Log to debug output
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Swagger
builder.Services.AddConfigSwagger();
//DBContext
builder.AddDbContext();
builder.Services.AddHttpContextAccessor();
//JWT
builder.AddJwtValidation();
//DI
builder.Services.AddUnitOfWork();
builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddIdentity();
builder.Services.AddAutoMapper();
//builder.Services.AddExceptionMiddleware();

builder.Services.AddCors(cors => cors.AddPolicy(
                            name: CorsConstant.PolicyName,
                            policy =>
                            {
                                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                            }
                        ));

var app = builder.Build();
app.AddApplicationConfig();
app.Run();
