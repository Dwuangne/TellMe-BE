using DotNetEnv;
using TellMe.API.Constants;
using TellMe.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // Xóa các provider mặc định nếu có
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Warning); // Chỉ log Warning trở lên
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
