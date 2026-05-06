using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;

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
        private readonly TelegramBotService _telegramService;
        private readonly ISikkerRepo _repo;

        public TelegramBotController(TelegramBotService telegramService, ISikkerRepo repo)
        {
            _telegramService = telegramService;
            _repo = repo;
        }

        /// <summary>
        /// Handle Telegram webhook updates: register chat ID if message contains Firebase ID, or check registration for other messages.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] TelegramMessage request)
        {
            if (request?.ChatId == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Invalid Telegram webhook payload.");
            }

            var telegramChatId = request.ChatId.ToString();
            var messageText = request.Message.Trim();

            // Check if message text is a Firebase ID (assume it's a valid Firebase ID format, e.g., length > 20 or contains specific chars)
            if (!string.IsNullOrWhiteSpace(messageText) && messageText.Length > 20) // Adjust threshold as needed
            {
                var updated = await _repo.UpdateUserChatIdAsync(messageText, telegramChatId);

                if(!updated)
                {    
                    return BadRequest("Firebase ID not found.");
                }       
                
                return Ok(new { message = "Chat ID registered successfully." });
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
        public async Task<IActionResult> SendMotionAlertMessage([FromBody] TelegramMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Description))
                return BadRequest("Description is required.");

            // Look up the user's registered chat ID before sending
            var user = await _repo.GetUserByFirebaseIdAsync(request.OwnerUid);
            if (user == null || string.IsNullOrWhiteSpace(user.TelegramChatId))
                return BadRequest("No Telegram chat ID registered for this user.");

            var message = $"Motion detected! {request.Description}";
            await _telegramService.SendMessageAsync(message, user.TelegramChatId);
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
                return BadRequest("ImageUrl is required.");

            // Pass ownerUid so the service can look up the correct chat ID
            await _telegramService.SendImageLinkAsync(request.ImageUrl, request.Description, request.OwnerUid);
            return Ok(new { message = "Motion alert link sent to Telegram." });
        }
    }

    public sealed class TelegramMessageRequest
    {
        public string Description { get; set; } = string.Empty;
        public string OwnerUid { get; set; } = string.Empty; 
    }

    /// <summary>Request model for sending links via Telegram.</summary>
    public sealed class TelegramLinkRequest
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerUid {get; set; } = string.Empty;
    }

    // public sealed class TelegramWebhookRequest
    // {
    //     public TelegramMessage? Message { get; set; }
    // }

    // public sealed class TelegramMessage
    // {
    //     public TelegramChat? Chat { get; set; }
    //     public string? Text { get; set; }
    // }

    // public sealed class TelegramChat
    // {
    //     public long Id { get; set; }
    // }
}

