using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.API.Services;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] double lat, [FromQuery] double lon)
        {
            var weather = await _weatherService.GetWeatherAsync(lat, lon);
            if (weather == null)
                return NotFound("Không lấy được dữ liệu thời tiết");

            return Ok(new
            {
                Location = weather.name,
                Temperature = weather.main.temp,
                Humidity = weather.main.humidity,
                Condition = weather.weather.FirstOrDefault()?.description
            });
        }
    }
}
