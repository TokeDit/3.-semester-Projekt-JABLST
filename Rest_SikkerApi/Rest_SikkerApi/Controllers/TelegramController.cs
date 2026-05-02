using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.interfaces;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("telegram")]
    public class TelegramController : ControllerBase
    {
        // Depend on ITelegramService abstraction, not concrete TelegramService
        private readonly ITelegramService _telegramService;

        // COMMIT  Replace static in-memory state with a dedicated state class to avoid
        // race conditions and make state testable and replaceable (e.g. with a DB later)
        private static long _lastChatId;
        private static string _lastMessage = "";
        private static DateTime _lastMessageTime;

        //  Inject ILogger for structured logging in controller actions
        private readonly ILogger<TelegramController> _logger;
    
     //  Inject ITelegramService instead of TelegramService
     //  Inject ILogger
        public TelegramController(ITelegramService telegramService, ILogger<TelegramController> logger)
        {
            _telegramService = telegramService;
            _logger = logger;
        }
        // Telegram webhook endpoint
        [HttpPost("update")]
        public async Task<IActionResult> ReceiveUpdate([FromBody] JsonElement update)
        {
            // COMMIT 4:
            // Upgrade signature to: public async Task<IActionResult> ReceiveUpdate([FromBody] JsonElement update, CancellationToken ct)
            // and pass ct to SendMessageAsync when ready

            // COMMIT 5: Guard against missing top-level "message" property
            // Telegram sends non-message updates (edited_message, callback_query, etc.) — these will throw without a guard
            if (!update.TryGetProperty("message", out var message))
            {
                _logger.LogWarning("Received Telegram update without a 'message' property — skipping.");
                return Ok(); // Always return 200 to Telegram or it will keep retrying
            }

            // COMMIT 5: Guard against missing "chat" property
            if (!message.TryGetProperty("chat", out var chat))
            {
                _logger.LogWarning("Received message without 'chat' property — skipping.");
                return Ok();
            }

            // COMMIT 5: Guard against missing "id" on chat
            if (!chat.TryGetProperty("id", out var chatIdElement))
            {
                _logger.LogWarning("Received chat without 'id' property — skipping.");
                return Ok();
            }

            long chatId = chatIdElement.GetInt64();

            // COMMIT 5: Guard against missing "text" — not all messages have text (e.g. photos, stickers)
            string text = message.TryGetProperty("text", out var textElement)
                ? textElement.GetString() ?? ""
                : "";

            // COMMIT 3: Log incoming message for observability
            _logger.LogInformation("Received Telegram message from chat {ChatId}: {Text}", chatId, text);

            // COMMIT 2: WARNING — static fields are shared across all requests and threads
            // This is not thread-safe and will lose data under concurrent requests
            // Future commit: replace with a scoped state service or database
            _lastChatId = chatId;
            _lastMessage = text;
            _lastMessageTime = DateTime.UtcNow;

            // COMMIT 6: Remove try/catch swallowing all exceptions into BadRequest
            // Let the ASP.NET exception middleware handle unexpected errors properly
            // Only catch what you can meaningfully handle
            try
            {
                await _telegramService.SendMessageAsync(chatId, $"You said: {text}");
            }
            catch (HttpRequestException ex)
            {
                // COMMIT 6: Catch only specific exceptions — HTTP failures sending to Telegram
                _logger.LogError(ex, "Failed to send Telegram reply to chat {ChatId}", chatId);
                // Still return 200 so Telegram does not retry the webhook
                return Ok();
            }

            // COMMIT 7: Return 200 OK always on webhook — Telegram will retry on non-200 responses
            return Ok();
        }

    }
}
