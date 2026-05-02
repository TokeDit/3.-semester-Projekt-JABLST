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

        // COMMIT 5: 
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(IConfiguration config, HttpClient httpClient)
        {
            _botToken = config["Telegram:BotToken"]
                        ?? throw new Exception("Telegram BotToken not found in configuration.");
            _httpClient = httpClient;
        }
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
    }
}
