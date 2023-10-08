using Microsoft.EntityFrameworkCore;
using Notification_API.Data;
using Notification_API.Extensions;
using Notification_API.Messaging;
using Notification_API.Services;
using Notification_API.Settings;
using Serilog;
using Shared.Logging;
using Shared.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

// Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddTransient(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

//Database Connection
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDatabaseConnectionString")));

//Singleton Implementation of the email service
var optionBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDatabaseConnectionString"));
builder.Services.AddSingleton(new EmailService(optionBuilder.Options, builder.Configuration));

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGridSettings"));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
app.UseSwagger();
app.UseSwaggerUI();

if (string.IsNullOrEmpty(isRunningInDocker) || !isRunningInDocker.ToLower().Equals("true"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.UseAzureServiceBusConsumer();
app.Run();

void ApplyMigration()
{
    using IServiceScope scope = app.Services.CreateScope();
    NotificationDbContext db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}