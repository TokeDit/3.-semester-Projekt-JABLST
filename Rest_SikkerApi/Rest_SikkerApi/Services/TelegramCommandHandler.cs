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

           
            // Add to appsettings.json: "Backend": { "BaseUrl": "http://localhost:5000" }
            _backendBaseUrl = config["Backend:BaseUrl"]
                ?? throw new InvalidOperationException("Backend BaseUrl not found in configuration.");
        }
        // COMMIT 5: 
        public async Task HandleCommandAsync(long chatId, string command, CancellationToken ct = default)
        {
            // COMMIT 6: Trim and normalize command before switching to avoid
            // whitespace or casing issues e.g. "/Help " failing to match
            var normalizedCommand = command.Trim().ToLower();

            _logger.LogInformation("Handling command '{Command}' for chat {ChatId}", normalizedCommand, chatId);

            switch (normalizedCommand)
            {
                case "/hjælp":
                case "/help":
                    await SendHelpAsync(chatId, ct);
                    break;

                case "/on":
                    // COMMIT 4: Use configured base URL instead of hardcoded localhost
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/on", ct);
                    break;

                case "/off":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/off", ct);
                    break;

                case "/status":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/status", ct);
                    break;

                default:
                    // COMMIT 7: Log unknown commands so you can discover what users are trying
                    _logger.LogWarning("Unknown command '{Command}' received from chat {ChatId}", normalizedCommand, chatId);
                    await _telegramService.SendMessageAsync(chatId,
                        "Ukendt kommando. Skriv /hjælp for at se muligheder.", ct);
                    break;
            }
        }
    }
}

