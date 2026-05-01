using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.Services;

namespace Rest_SikkerApi.Controllers
{
    /// <summary>
    /// TelegramBotController exposes standalone endpoints for manual Telegram messaging.
    /// These endpoints can be used independently of the image upload flow.
    /// 
    /// Example: Send a custom alert without an image upload.
    /// 
    /// Endpoints:
    /// - POST /api/TelegramBot/message : Send a text message
    /// - POST /api/TelegramBot/link : Send a message with a link (dashboard URL)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TelegramBotController : ControllerBase
    {
        private readonly TelegramService _telegramService;

        public TelegramBotController(TelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        /// <summary>
        /// Send a custom text message to Telegram.
        /// Request body: plain text string (e.g., "Motion detected in front door")
        /// 
        /// Returns: 200 OK if message sent successfully.
        /// </summary>
        [HttpPost("message")]
        public async Task<IActionResult> SendMotionAlertMessage([FromBody] string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest("Description is required.");
            }

            string message = $"Motion detected! {description}";
            await _telegramService.SendMessageAsync(message);
            return Ok(new { message = "Motion alert message sent to Telegram." });
        }

        /// <summary>
        /// Send a message with a clickable link to Telegram.
        /// 
        /// Request body JSON:
        /// {
        ///   "ImageUrl": "https://your-dashboard-url.com/dashboard",
        ///   "Description": "Optional description (e.g., 'Camera 1 alert')"
        /// }
        /// 
        /// Returns: 200 OK if message sent successfully.
        /// </summary>
        [HttpPost("link")]
        public async Task<IActionResult> SendMotionAlertLink([FromBody] TelegramLinkRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                return BadRequest("ImageUrl is required.");
            }

            await _telegramService.SendImageLinkAsync(request.ImageUrl, request.Description);
            return Ok(new { message = "Motion alert link sent to Telegram." });
        }
    }

    /// <summary>Request model for sending links via Telegram.</summary>
    public sealed class TelegramLinkRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

