using Dapper;
using Journey_of_faith.Api.middlewares;
using Journey_of_faith.Application;
using Journey_of_faith.Infrastructure;
using Journey_of_faith.Infrastructure.context;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using OfficeOpenXml;
using Asp.Versioning;
using Journey_of_faith.Api.authorization;
using Journey_of_faith.Infrastructure.scheduling;
using Microsoft.AspNetCore.Authorization;
using Serilog;


var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    foreach (var line in File.ReadLines(envFile))
    {
        var entry = line.Trim();
        if (entry.Length == 0 || entry.StartsWith('#')) continue;
        var separator = entry.IndexOf('=');
        if (separator <= 0) continue;
        var name = entry[..separator].Trim();
        var value = entry[(separator + 1)..].Trim();
        if (Environment.GetEnvironmentVariable(name) is null)
            Environment.SetEnvironmentVariable(name, value);
    }
}

var builder = WebApplication.CreateBuilder(args);
const string logOutputTemplate =
    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}";

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate: logOutputTemplate)
    .WriteTo.File(
        Path.Combine("logs", "journey-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        outputTemplate: logOutputTemplate));

SqlMapper.AddTypeHandler(new GuidTypeHandler());

builder.Services.AddHttpContextAccessor();

string[] origins = builder
    .Configuration.GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowFrontend", policy =>
        policy
            .WithOrigins(
                origins
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// Add services to the container.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the JWT token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});
// builder.Services.AddFirebaseService(builder.Configuration);
builder.Services.AddSingleton<IAuthorizationPolicyProvider, UserPermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, UserPermissionHandler>();

builder.Services.AddInfrastructure(builder.Configuration);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddFirebaseService(builder.Configuration);
}
builder.Services.AddRegisterService(builder.Configuration);
builder.Services.AddNotificationScheduling(builder.Configuration);

// builder.Services.AddNotificationSchedulingAuthorization();

builder.Services.AddApplication();

builder.Services.AddAutoMapperConfig();

builder.Services.AddControllers();
builder.Services.AddHttpClient("Gemini", client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader()  
    );
}).AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

ExcelPackage.License.SetNonCommercialOrganization("JourneyOfFaith");

// builder.Services.AddScoped<CustomHttpResponseFormatter>();

var app = builder.Build();


// ==========================================================
// DATABASE CHECK
// ==========================================================
// Khi chạy Integration Test với Environment = "Testing",
// không thực hiện CanConnectAsync() ở đây.
// CustomWebApplicationFactory sẽ cấu hình InMemory Database.
// ==========================================================

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _ = dbContext.Model;

        await dbContext.Database.CanConnectAsync();
    }
}


app.UseSerilogRequestLogging();


// ==========================================================
// HTTP REQUEST PIPELINE
// ==========================================================

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});

app.UseCors("allowFrontend");

app.UseExceptionHandler();

// app.UseHttpsRedirection();


// ==========================================================
// UPLOADS
// ==========================================================

var currentDirectoryFile =
    System.IO.Path.Combine(
        Directory.GetCurrentDirectory(),
        "uploads"
    );

Directory.CreateDirectory(currentDirectoryFile);

var churchUploadDirectory = System.IO.Path.Combine(
    currentDirectoryFile,
    "churchs"
);

Directory.CreateDirectory(churchUploadDirectory);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(currentDirectoryFile),
    RequestPath = "/uploads"
});


// ==========================================================
// DEVELOPMENT
// ==========================================================

// ==========================================================
// AUTHENTICATION / AUTHORIZATION
// ==========================================================

app.UseAuthentication();

app.UseAuthorization();


// ==========================================================
// CONTROLLERS
// ==========================================================

app.MapControllers();


// ==========================================================
// RUN
// ==========================================================

app.Run();


// ==========================================================
// GUID TYPE HANDLER
// ==========================================================

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }

    public override Guid Parse(object value)
    {
        return Guid.Parse(value.ToString()!);
    }
}
