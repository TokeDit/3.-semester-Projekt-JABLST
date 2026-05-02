using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("telegram")]
public class TelegramController : ControllerBase
{
    // COMMIT 1: Depend on ITelegramService abstraction, not concrete TelegramService
    private readonly ITelegramService _telegramService;

    // COMMIT 2: Inject ITelegramCommandHandler to route commands properly
    private readonly ITelegramCommandHandler _commandHandler;

    // COMMIT 3: Replace static in-memory state with a dedicated state class to avoid
    // race conditions and make state testable and replaceable (e.g. with a DB later)
    private static long _lastChatId;
    private static string _lastMessage = "";
    private static DateTime _lastMessageTime;

    // COMMIT 4: Inject ILogger for structured logging in controller actions
    private readonly ILogger<TelegramController> _logger;

    // COMMIT 2: Add ITelegramCommandHandler to constructor
    public TelegramController(
        ITelegramService telegramService,
        ITelegramCommandHandler commandHandler,
        ILogger<TelegramController> logger)
    {
        _telegramService = telegramService;
        _commandHandler = commandHandler;
        _logger = logger;
    }

    [HttpPost("update")]
    public async Task<IActionResult> ReceiveUpdate([FromBody] JsonElement update, CancellationToken ct)
    {
        // COMMIT 5: Guard against missing top-level "message" property
        if (!update.TryGetProperty("message", out var message))
        {
            _logger.LogWarning("Received Telegram update without a 'message' property — skipping.");
            return Ok();
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

        // COMMIT 5: Guard against missing "text" — not all messages have text
        string text = message.TryGetProperty("text", out var textElement)
            ? textElement.GetString() ?? ""
            : "";

        // COMMIT 4: Log incoming message
        _logger.LogInformation("Received Telegram message from chat {ChatId}: {Text}", chatId, text);

        // COMMIT 3: Save in-memory state (not thread-safe — replace with scoped service later)
        _lastChatId = chatId;
        _lastMessage = text;
        _lastMessageTime = DateTime.UtcNow;

        try
        {
            // COMMIT 2: Route to command handler instead of echoing back
            await _commandHandler.HandleCommandAsync(chatId, text, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to handle command for chat {ChatId}", chatId);
            // Still return 200 so Telegram does not retry
            return Ok();
        }

        return Ok();
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            lastChatId = _lastChatId,
            lastMessage = _lastMessage,
            // COMMIT 6: Return ISO 8601 UTC timestamp string
            lastMessageTime = _lastMessageTime == default
                ? null
                : (string?)_lastMessageTime.ToString("o")
        });
    }
}