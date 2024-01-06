using Destructurama;
using Dissertation.Application.Mapping;
using Dissertation.Infrastructure.Context;
using Dissertation_API.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .Enrich.FromLogContext()
    .Destructure.UsingAttributes()
    .Enrich.WithCorrelationId()
    .ReadFrom.Configuration(context.Configuration));
builder.Services.ConfigureMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApiVersioning();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMapster();

WebApplication app = builder.Build();

if (string.IsNullOrEmpty(isRunningInDocker) || !isRunningInDocker.ToLower().Equals("true"))
{
    app.UseHttpsRedirection();
}
app.UseInfrastructure(app.Configuration);
app.ConfigureSwagger();
app.UseAzureServiceBusConsumer();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using IServiceScope scope = app.Services.CreateScope();
    DissertationDbContext db = scope.ServiceProvider.GetRequiredService<DissertationDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}