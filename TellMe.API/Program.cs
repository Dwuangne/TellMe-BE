using DotNetEnv;
using System.Reflection;
using System.Text.Json.Serialization;
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
    // Thay đổi mức log cho development
    if (builder.Environment.IsDevelopment())
    {
        logging.SetMinimumLevel(LogLevel.Information);
    }
    else 
    {
        logging.SetMinimumLevel(LogLevel.Warning); // Chỉ log Warning trở lên ở môi trường production
    }
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

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Đảm bảo middleware cho xử lý ngoại lệ phải đặt trước các middleware khác
app.UseMiddleware<ExceptionMiddleware>();

// Sau đó gọi ApplicationConfig
app.AddApplicationConfig();
app.Run();
