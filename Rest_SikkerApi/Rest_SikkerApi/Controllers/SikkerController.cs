using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TelegramBotController : ControllerBase
{
    private readonly TelegramService _telegramService;

    public TelegramBotController(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    /// <summary>
    /// Sends a motion alert message to the configured Telegram chat.
    /// </summary>
    /// <param name="description">Description of the event or camera location.</param>
    [HttpPost("send-message")]
    [ProducesResponseType(StatusCodes.Status200OK)]    // Success
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Missing description
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Unexpected failure
    public async Task<IActionResult> SendMotionAlertMessage([FromQuery] string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return BadRequest(new { error = "Description is required." });

        try
        {
            string message = $"Motion detected! {description}";
            await _telegramService.SendMessage(message);
            return Ok(new { message = "Motion alert message sent to Telegram." });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Sends a motion alert including a photo to the Telegram chat.
    /// </summary>
    /// <param name="description">Description of the event or camera location.</param>
    /// <param name="imagePath">Path to the captured image file.</param>
    [HttpPost("send-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]    // Success
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Missing or invalid imagePath
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Unexpected failure
    public async Task<IActionResult> SendMotionAlertWithPhoto([FromQuery] string description, [FromQuery] string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !System.IO.File.Exists(imagePath))
            return BadRequest(new { error = "Valid imagePath is required." });

        try
        {
            string caption = $"Motion detected! {description}";
            await _telegramService.SendPhoto(imagePath, caption);
            return Ok(new { message = "Motion alert with photo sent to Telegram." });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}