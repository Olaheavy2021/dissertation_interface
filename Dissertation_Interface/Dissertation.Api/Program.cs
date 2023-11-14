using Dissertation_API.Extensions;
using Dissertation.Application.Mapping;
using Dissertation.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
builder.Services.ConfigureMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureApiVersioning();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMapster();

WebApplication app = builder.Build();
app.ConfigureSwagger();
if (string.IsNullOrEmpty(isRunningInDocker) || !isRunningInDocker.ToLower().Equals("true"))
{
    app.UseHttpsRedirection();
}
app.UseInfrastructure(app.Configuration);
app.Run();