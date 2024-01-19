using System.Diagnostics.CodeAnalysis;
using Destructurama;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserManagement_API.Data;
using UserManagement_API.Extensions;
using UserManagement_API.Middleware;
using UserManagement_API.Middleware.Correlation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

// Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .Enrich.FromLogContext()
    .Destructure.UsingAttributes()
    .Enrich.WithCorrelationId()
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder => builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

//Register custom services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

WebApplication app = builder.Build();

//Middleware
app.UseMiddleware<SwaggerBasicAuthMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<UserDetailsMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("all");

app.UseSerilogRequestLogging();

if (string.IsNullOrEmpty(isRunningInDocker) || !isRunningInDocker.ToLower().Equals("true"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/healthz");
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using IServiceScope scope = app.Services.CreateScope();
    UserDbContext db = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

[ExcludeFromCodeCoverage]
public partial class Program { }