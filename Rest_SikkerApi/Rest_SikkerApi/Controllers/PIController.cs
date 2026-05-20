using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.interfaces;
namespace Rest_SikkerApi;

[ApiController]
[Route("api/[controller]")]
public class PIController : ControllerBase
{
    private readonly ISikkerRepo _repo;
    private static DateTime? _lastHeartBeat;
    private readonly TelegramBotService _telegramService;
    private readonly IFirebaseHandler _firebaseHandler;



    public PIController(ISikkerRepo repo, TelegramBotService telegramService, IFirebaseHandler firebaseHandler)
    {
        _repo = repo;
        _telegramService = telegramService;
        _firebaseHandler = firebaseHandler;
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

            string firebaseUid = await _firebaseHandler.GetFirebaseUid();
            
            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return BadRequest("No firebase id found");
            }

            await _repo.SaveImageAsync(image);
            var dashboardUrl = "https://sikkerheds-app-jablst-f0ewdphzhsf0hqcr.swedencentral-01.azurewebsites.net/home";

            await _telegramService.SendImageLinkAsync(dashboardUrl, image.Description, firebaseUid);

            return Ok(image);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Image analysis failed.\n" + ex.Message,
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
        var threshold = TimeSpan.FromSeconds(30);
        var isAlive = _lastHeartBeat.HasValue && (DateTime.UtcNow - _lastHeartBeat.Value) < threshold;

        return Ok(new
        {
            lastSeen = _lastHeartBeat,
            isAlive = isAlive
        });

    }



}
