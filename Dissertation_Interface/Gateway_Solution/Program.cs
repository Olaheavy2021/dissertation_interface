using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Destructurama;
using Gateway_Solution.Extensions;
using Gateway_Solution.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

if (builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
{
    builder.Configuration.AddJsonFile("ocelot.Production.json", optional: false, reloadOnChange: true);
}
else if (!string.IsNullOrEmpty(isRunningInDocker) && isRunningInDocker.ToLower().Equals("true"))
{
    builder.Configuration.AddJsonFile("ocelot.Docker.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

builder.Services.AddOcelot(builder.Configuration);
builder.Services.SetupAuthentication(builder.Configuration);

// Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .Enrich.FromLogContext()
    .Destructure.UsingAttributes()
    .Enrich.WithCorrelationId()
    .ReadFrom.Configuration(context.Configuration));
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.MapGet("/", () => "Welcome to the SHU Dissertation API Gateway");
app.UseRouting();
app.ConfigureEndpoints();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
OcelotPipelineConfiguration configuration = OcelotConfigurator.CreateConfiguration();
app.UseOcelot(configuration).GetAwaiter().GetResult();
app.Run();