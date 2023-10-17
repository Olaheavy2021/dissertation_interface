using Microsoft.AspNetCore.Mvc;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Notification_API.Services.IServices;
using Shared.Helpers;

namespace Notification_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly AuditLogService _auditLogService;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, AuditLogService auditLogService)
        {
            this._logger = logger;
            this._auditLogService = auditLogService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get() => Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
            .ToArray();

        [HttpPost("TestEmail")]
        public async Task<IActionResult> TestEmail([FromQuery] PaginationParameters parameters)
        {
            ResponseDto<PagedList<AuditLog>> response = await this._auditLogService.GetListOfAuditLogs(parameters);
            return Ok(response);
        }
    }
}