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
            if (image == null || string.IsNullOrEmpty(image.ImageData))
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

        //[HttpGet]
        //public IActionResult<IEnumerable<>> Get()
        //{
        //    return Ok();
        //}
    }
}
