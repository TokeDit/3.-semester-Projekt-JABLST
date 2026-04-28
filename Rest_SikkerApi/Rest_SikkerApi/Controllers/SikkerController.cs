using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;


namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SikkerController : ControllerBase
    {

        private readonly SikkerRepo _repo;
        private readonly ILogger<SikkerController> _logger;

        public SikkerController(ILogger<SikkerController> logger, SikkerRepo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        //[HttpGet]
        //public IActionResult<IEnumerable<>> Get()
        //{
        //    return Ok();
        //}
    }
}
