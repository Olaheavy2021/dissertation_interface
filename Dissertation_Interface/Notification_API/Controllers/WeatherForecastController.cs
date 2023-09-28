using Microsoft.AspNetCore.Mvc;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;

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
        private readonly EmailService _emailService;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, EmailService emailService)
        {
            this._logger = logger;
            this._emailService = emailService;
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
        public async Task<IActionResult> TestEmail([FromBody] TestEmailDto request)
        {
            ResponseDto response = await this._emailService.RegisterAdminUserEmailAndLog(request.emailBody, request.email);
            return Ok(response);
        }
    }
}