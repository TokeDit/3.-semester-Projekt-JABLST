using Rest_SikkerApi.interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Rest_SikkerApi.Services
{
    public class TelegramCommandHandler : interfaces.ITelegramCommandHandler
    {

        private readonly ITelegramService _telegramService;

        //  Fix HttpClient lifecycle — inject via DI instead of new HttpClient()
        private readonly HttpClient _httpClient;

        //  Inject ILogger for structured logging
        private readonly ILogger<TelegramCommandHandler> _logger;

        //  Extract backend base URL from hardcoded localhost to configuration
        private readonly string _backendBaseUrl;


        public TelegramCommandHandler(
               ITelegramService telegramService,
               HttpClient httpClient,
               ILogger<TelegramCommandHandler> logger,
               IConfiguration config)
        {
            _telegramService = telegramService;
            _httpClient = httpClient;
            _logger = logger;

            // COMMIT 4:
            // Add to appsettings.json: "Backend": { "BaseUrl": "http://localhost:5000" }
            _backendBaseUrl = config["Backend:BaseUrl"]
                ?? throw new InvalidOperationException("Backend BaseUrl not found in configuration.");
        }
    }
}

