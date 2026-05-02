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
            // COMMIT 8: G
            if (chatId == 0)
                throw new ArgumentException("Invalid chat ID.", nameof(chatId));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            // COMMIT 9: Use extracted base URL constant
            var url = $"{TelegramBaseUrl}/bot{_botToken}/sendMessage";

            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "Markdown"
            };
        }
}
