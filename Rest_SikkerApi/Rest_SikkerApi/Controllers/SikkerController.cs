using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SikkerController : ControllerBase
    {

        private readonly SikkerRepo _repo;
        private readonly ILogger<SikkerController> _logger;
        private readonly TelegramService _telegramService;
        private readonly string _dashboardUrl;

        public SikkerController(ILogger<SikkerController> logger, SikkerRepo repo, TelegramService telegramService, IConfiguration configuration)
        {
            _logger = logger;
            _repo = repo;
            _telegramService = telegramService;
            _dashboardUrl = configuration["DashboardUrl"] ?? "https://localhost:5173/dashboard";
        }

        // POST: /Sikker/UploadImage
        [HttpPost("UploadImage", Name = "UploadImage")]
        public async Task<IActionResult> UploadImage([FromBody] Image image)
        {
            if (image == null || image.ImageData == null || image.ImageData.Length == 0)
                return BadRequest("Image object is null or Imagedata is missing");

            if (string.IsNullOrWhiteSpace(image.Id))
            {
                image.Id = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrWhiteSpace(image.TimeStamp))
            {
                image.TimeStamp = DateTime.UtcNow.ToString("o");
            }

            _logger.LogInformation("Received image with ID: {ImageId} and Type: {ImageType}", image.Id, image.ImageType);

            try
            {
                byte[] imageBytes = image.GetImageBytes() ?? Array.Empty<byte>();

                await _repo.SaveImageAsync(image);
                var dashboardUrl = _dashboardUrl;

                await _telegramService.SendImageLinkAsync(dashboardUrl, image.Description);

                return Ok(new { message = "Image received successfully", size = imageBytes.Length, dashboardUrl });
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 string");
            }
        }

        // GET: /Sikker/Images
        [HttpGet("Images", Name = "Images")]
        public async Task<ActionResult<IEnumerable<Image>>> Get()
        {
            var images = await _repo.GetAllImagesAsync();
            if (images == null || !images.Any())
                return NotFound("No images found");

            return Ok(images);
        }

        // GET: /Sikker/Image/{id}
        [HttpGet("Image/{id}", Name = "GetImageById")]
        public async Task<IActionResult> GetImageById(string id)
        {
            var image = await _repo.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(image.ImageType) ? "application/octet-stream" : image.ImageType;
            return File(image.ImageData, contentType, $"{id}.jpg");
        }

        //[HttpGet]
        //public IActionResult<IEnumerable<>> Get()
        //{
        //    return Ok();
        //}
        // NEW: System Status Endpoint
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "online" });
        }
    }
}