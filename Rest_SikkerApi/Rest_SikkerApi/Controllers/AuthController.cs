using Microsoft.AspNetCore.Mvc;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        //[HttpGet()]
        //public IEnumerable<WeatherForecast> Get()
        //{
            
        //}
    }
}
