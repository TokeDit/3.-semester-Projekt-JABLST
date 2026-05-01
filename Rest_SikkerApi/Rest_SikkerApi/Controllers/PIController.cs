using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
namespace Rest_SikkerApi;
using Rest_SikkerApi.models;

[ApiController]
[Route("api/[controller]")]
public class PIController : ControllerBase
{
    private readonly SikkerRepo m_repo;

    public PIController(SikkerRepo repo)
    {
        m_repo = repo;
    }
    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Post([FromBody] Image image)
    {
        try
        {
            if (image is null || image.ImageData.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            // tilgiver FirebaseUid i både HttpContext og User.Claims for at sikre kompatibilitet med forskellige autentificeringsmetoder
            var firebaseUid = HttpContext.Items["FirebaseUid"] as string ?? User.FindFirst("firebase_uid")?.Value ?? string.Empty;



            // Create Image entity, set OwnerUid and save
            var imageEntity = new Image
            {
                Id = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.UtcNow.ToString("o"),
                ImageType = image.ImageType,
                ImageData = image.ImageData,
                Description = image.Description,
                OwnerUid = firebaseUid
            };

            await m_repo.SaveImageAsync(imageEntity);


            

            return Ok(imageEntity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Image analysis failed.",
                error = ex.Message
            });
        }
    }
}
