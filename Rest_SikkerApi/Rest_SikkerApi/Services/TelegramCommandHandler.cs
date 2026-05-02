using Rest_SikkerApi.interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Rest_SikkerApi.Services
{
    public class TelegramCommandHandler : interfaces.ITelegramCommandHandler
    {
        // COMMIT 1: 
        private readonly ITelegramService _telegramService;

        // COMMIT 2: Fix HttpClient lifecycle — inject via DI instead of new HttpClient()
        private readonly HttpClient _httpClient;

        // COMMIT 3: Inject ILogger for structured logging
        private readonly ILogger<TelegramCommandHandler> _logger;

        // COMMIT 4: Extract backend base URL from hardcoded localhost to configuration
        private readonly string _backendBaseUrl;

        // COMMIT 1: Inject ITelegramService instead of concrete TelegramService
        // COMMIT 2: Inject HttpClient via DI
        // COMMIT 3: Inject ILogger
        // COMMIT 4: Inject IConfiguration to read backend base URL
    }
}

