using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;
using System.Threading.Tasks;


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

        // POST: /Sikker/UploadImage
        [HttpPost (Name = "UploadImage")]
        public async Task<IActionResult> UploadImage([FromBody] Image image)
        {
            if (image == null || image.ImageData == null || image.ImageData.Length == 0)
                return BadRequest("Image object is null or Imagedata is missing");

            _logger.LogInformation("Received image with ID: {ImageId} and Type: {ImageType}", image.Id, image.ImageType);

            try
            {
                // Decode Base64 string to bytes
                byte[] imageBytes = image.GetImageBytes() ?? Array.Empty<byte>();

                // Now you can save imageBytes to database, file system, etc.
                // For example:
                // await _repository.SaveImageAsync(image.Id, imageBytes, image.ImageType);
                await _repo.SaveImageAsync(image);

                return Ok(new { message = "Image received successfully", size = imageBytes.Length });
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 string");
            }
        }

        // GET: /Sikker/Images
        [HttpGet(Name = "Images")]
        public async Task<ActionResult<IEnumerable<Image>>> Get()
        {
            var images = await _repo.GetAllImagesAsync();
            if (images == null || !images.Any())
                return NotFound("No images found");

            return Ok(images);
        }

        //[HttpGet]
        //public IActionResult<IEnumerable<>> Get()
        //{
        //    return Ok();
        //}

        // POST: /Sikker/on
        [HttpPost("on")]
        public IActionResult TurnOn()
        {
            _repo.SetSystemState(true);
            _logger.LogInformation("System turned ON");
            return Ok(new { status = "online", message = "System turned on" });
        }

        // POST: /Sikker/off
        [HttpPost("off")]
        public IActionResult TurnOff()
        {
            _repo.SetSystemState(false);
            _logger.LogInformation("System turned OFF");
            return Ok(new { status = "offline", message = "System turned off" });
        }

        // Update existing status endpoint to use real state
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            bool isOnline = _repo.GetSystemState();
            return Ok(new { status = isOnline ? "online" : "offline" });
        }
        //To Test and Call GET /Secure/ping and measure round-trip time
        [HttpGet("ping")]

        public IActionResult Ping() {
            return Ok(new
            {
                message = "pong",
                timestamp = DateTime.UtcNow,
            });
        }

    }
}
