using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Notification_API.Data;
using Notification_API.Extensions;
using Notification_API.Middleware;
using Notification_API.Middleware.Correlation;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

// Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.ConfigureApiVersioning();
builder.Services.SetupAuthentication(builder.Configuration);
builder.Services.SetupSwaggerBearerAuthentication();


WebApplication app = builder.Build();

//Middleware
app.ConfigureCors();
app.UseMiddleware<SwaggerBasicAuthMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
if (string.IsNullOrEmpty(isRunningInDocker) || !isRunningInDocker.ToLower().Equals("true"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.ConfigureEndpoints();
app.ApplyMigration();
app.UseAzureServiceBusConsumer();
app.Run();