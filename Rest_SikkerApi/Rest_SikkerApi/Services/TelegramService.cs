using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Rest_SikkerApi.Services
{
    public class TelegramService
    {
        private const string TelegramBaseUrl = "https://api.telegram.org";

        private readonly string _botToken;
        private readonly HttpClient _httpClient;

        
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(IConfiguration config, HttpClient httpClient, ILogger<TelegramService> logger)
        {
           
            _botToken = config["Telegram:BotToken"]
                        ?? throw new InvalidOperationException("Telegram BotToken not found in configuration.");

            //  Use injected HttpClient instead of new HttpClient()
            _httpClient = httpClient;

            //  Assign injected logger
            _logger = logger;
        }

        public async Task SendMessageAsync(long chatId, string message, CancellationToken ct = default)
        {
           
            if (chatId == 0)
                throw new ArgumentException("Invalid chat ID.", nameof(chatId));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            //  Use extracted base URL constant
            var url = $"{TelegramBaseUrl}/bot{_botToken}/sendMessage";

            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "Markdown"
            };
            // COMMIT 11: 
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(payload, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // COMMIT 5: Log outgoing message for observability
            _logger.LogInformation("Sending Telegram message to chat {ChatId}", chatId);

            // COMMIT 7: Wrap HTTP call in try/catch for structured error handling
            try
            {
                // COMMIT 2: Check HTTP response and throw on failure instead of silently swallowing errors
                // COMMIT 4: Pass CancellationToken to PostAsync
                var response = await _httpClient.PostAsync(url, content, ct);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully sent Telegram message to chat {ChatId}", chatId);
            }
            catch (HttpRequestException ex)
            {
                // COMMIT 7: Log and rethrow with context on network/HTTP failure
                _logger.LogError(ex, "Failed to send Telegram message to chat {ChatId}", chatId);
                throw;
            }
        }
    }
}