using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.repos;

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
    /// - POST /api/TelegramBot/webhook : Handle Telegram webhook updates
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TelegramBotController : ControllerBase
    {
        private readonly TelegramService _telegramService;
        private readonly ISikkerRepo _repo;

        public TelegramBotController(TelegramService telegramService, ISikkerRepo repo)
        {
            _telegramService = telegramService;
            _repo = repo;
        }

        /// <summary>
        /// Handle Telegram webhook updates: register chat ID if message contains Firebase ID, or check registration for other messages.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] TelegramWebhookRequest request)
        {
            if (request?.Message?.Chat == null || string.IsNullOrWhiteSpace(request.Message.Text))
            {
                return BadRequest("Invalid Telegram webhook payload.");
            }

            var telegramChatId = request.Message.Chat.Id.ToString();
            var messageText = request.Message.Text.Trim();

            // Check if message text is a Firebase ID (assume it's a valid Firebase ID format, e.g., length > 20 or contains specific chars)
            if (!string.IsNullOrWhiteSpace(messageText) && messageText.Length > 20) // Adjust threshold as needed
            {
                // Try to register: find user by Firebase ID and save chat ID
                var user = await _repo.GetUserByFirebaseIdAsync(messageText);
                if (user != null)
                {
                    await _repo.UpdateUserChatIdAsync(user.OwnerUid, telegramChatId);
                    return Ok(new { message = "Chat ID registered successfully." });
                }
                else
                {
                    return BadRequest("Firebase ID not found.");
                }
            }
            else
            {
                // For other messages, check if chat ID is registered
                var user = await _repo.GetUserByChatIdAsync(telegramChatId);
                if (user != null)
                {
                    // User is registered, process the message (e.g., acknowledge)
                    return Ok(new { message = "Message received from registered user." });
                }
                else
                {
                    return BadRequest("Chat ID not registered.");
                }
            }
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

    public sealed class TelegramWebhookRequest
    {
        public TelegramMessage? Message { get; set; }
    }

    public sealed class TelegramMessage
    {
        public TelegramChat? Chat { get; set; }
        public string? Text { get; set; }
    }

    public sealed class TelegramChat
    {
        public long Id { get; set; }
    }
}

