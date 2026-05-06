using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
namespace Rest_SikkerApi;
using Rest_SikkerApi.models;
using Rest_SikkerApi.Services;

[ApiController]
[Route("api/[controller]")]
public class PIController : ControllerBase
{
    private readonly ISikkerRepo _repo;
    private static DateTime? _lastHeartBeat;
    private readonly TelegramBotService _telegramService;


    
    public PIController(ISikkerRepo repo, TelegramBotService telegramService)
    {
        _repo = repo;
        _telegramService = telegramService;
    }

    [HttpPost]
    [ProducesResponseType (StatusCodes.Status200OK)]
    [ProducesResponseType (StatusCodes.Status400BadRequest)]
    [ProducesResponseType (StatusCodes.Status500InternalServerError)]
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
            var firebaseUid = HttpContext.Items["FirebaseUid"] as string
                ?? User.FindFirst("firebase_uid")?.Value
                ?? image.OwnerUid
                ?? string.Empty;



            // Create Image entity, set OwnerUid and save
            var imageEntity = new Image
            {
                TimeStamp = image.TimeStamp,
                ImageType = image.ImageType,
                ImageData = image.ImageData,
                Description = image.Description,
                OwnerUid = image.OwnerUid,
                Confidence = image.Confidence
            };

            await _repo.SaveImageAsync(imageEntity);
            if(!string.IsNullOrWhiteSpace(firebaseUid))
            {
                var user = await _repo.GetUserByFirebaseIdAsync(firebaseUid);
                if(user != null && !string.IsNullOrWhiteSpace(user.TelegramChatId))
                {
                    var dashboardUrl = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/dashboard";
                    await _telegramService.SendImageLinkAsync(dashboardUrl, image.Description, user.OwnerUid);
                }
            }
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
    [HttpPost("heartbeat")]
    public IActionResult HeartBeat([FromBody] HeartBeatDto body)
    {
        _lastHeartBeat = DateTime.UtcNow;
        return Ok();
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var threshold = TimeSpan.FromMinutes(2);
        var isAlive = _lastHeartBeat.HasValue && (DateTime.UtcNow - _lastHeartBeat.Value) < threshold;

        return Ok(new
        {
            lastSeen = _lastHeartBeat,
            isAlive = isAlive
        });

    }



}
