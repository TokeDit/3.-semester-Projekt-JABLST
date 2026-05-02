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
            // COMMIT 6: 
            _botToken = config["Telegram:BotToken"]
                        ?? throw new InvalidOperationException("Telegram BotToken not found in configuration.");

            // COMMIT 1: Use injected HttpClient instead of new HttpClient()
            _httpClient = httpClient;

            // COMMIT 5: Assign injected logger
            _logger = logger;
        }

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }
}
