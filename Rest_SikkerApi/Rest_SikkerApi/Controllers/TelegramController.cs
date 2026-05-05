using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rest_SikkerApi.data;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.models;
using System.Net.NetworkInformation;
using System.Text.Json;

[ApiController]
[Route("telegram")]
public class TelegramController : ControllerBase
{
    // Depend on ITelegramService abstraction, not concrete TelegramService
    private readonly ITelegramService _telegramService;

    //  Inject ITelegramCommandHandler to route commands properly
    private readonly ITelegramCommandHandler _commandHandler;

 
    //  Inject ILogger for structured logging in controller actions
    private readonly ILogger<TelegramController> _logger;

    //  Add ITelegramCommandHandler to constructor
    private readonly AppDbContext _db;

    public TelegramController(
        ITelegramService telegramService,
        ITelegramCommandHandler commandHandler,
        ILogger<TelegramController> logger,
        AppDbContext db)
    {
        _telegramService = telegramService;
        _commandHandler = commandHandler;
        _logger = logger;
        _db = db;
    }

    [HttpPost("update")]
    public async Task<IActionResult> ReceiveUpdate([FromBody] JsonElement update, CancellationToken ct)
    {
        _logger.LogInformation("RAW Telegram update: {Json}", update.ToString());

        // FIX: Ignore updates that are not JSON objects
        if (update.ValueKind != JsonValueKind.Object)
        {
            _logger.LogWarning("Received non-object update: {Json}", update.ToString());
            return Ok();
        }

        //  Guard against missing top-level "message" property
        if (!update.TryGetProperty("message", out var message))
        {
            _logger.LogWarning("Received Telegram update without a 'message' property — skipping.");
            return Ok();
        }

        // Guard against missing "chat" property
        if (!message.TryGetProperty("chat", out var chat))
        {
            _logger.LogWarning("Received message without 'chat' property — skipping.");
            return Ok();
        }

        //  Guard against missing "id" on chat
        if (!chat.TryGetProperty("id", out var chatIdElement))
        {
            _logger.LogWarning("Received chat without 'id' property — skipping.");
            return Ok();
        }

        long chatId = chatIdElement.GetInt64();

        //  Guard against missing "text" — not all messages have text
        string text = message.TryGetProperty("text", out var textElement)
            ? textElement.GetString() ?? ""
            : "";

        //  Log incoming message
        // REPLACE static field assignments with DB write
        _db.TelegramMessages.Add(new TelegramMessage
        {
            ChatId = chatId,
            Message = text,
            ReceivedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);

        try
        {
            _db.TelegramMessages.Add(new TelegramMessage
            {
                ChatId = chatId,
                Message = text,
                ReceivedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            // Log but don't crash — DB might not be available locally
            _logger.LogWarning(ex, "Could not save message to DB — continuing without persistence.");
        }

        try
        {
            await _commandHandler.HandleCommandAsync(chatId, text, ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to handle command for chat {ChatId}", chatId);
            return Ok();
        }

        return Ok();
    }
    

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var last = await _db.TelegramMessages
            .OrderByDescending(m => m.ReceivedAt)
            .FirstOrDefaultAsync();

        return Ok(new
        {
            lastChatId = last?.ChatId ?? 0,
            lastMessage = last?.Message ?? "",
            lastMessageTime = last?.ReceivedAt.ToString("o")
        });
    }
}