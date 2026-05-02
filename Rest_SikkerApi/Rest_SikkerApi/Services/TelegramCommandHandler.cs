using Rest_SikkerApi.interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Rest_SikkerApi.Services
{
   
    public class TelegramCommandHandler : ITelegramCommandHandler
    {
        //  Depend on ITelegramService abstraction, not concrete TelegramService
        private readonly ITelegramService _telegramService;

        // Fix HttpClient lifecycle — inject via DI instead of new HttpClient()
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

              public async Task HandleCommandAsync(long chatId, string command, CancellationToken ct = default)
        {
            //  Trim and normalize command before switching to avoid
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
                    //  Use configured base URL instead of hardcoded localhost
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/on", ct);
                    break;

                case "/off":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/off", ct);
                    break;

                case "/status":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/status", ct);
                    break;

                default:
                    //  Log unknown commands so you can discover what users are trying
                    _logger.LogWarning("Unknown command '{Command}' received from chat {ChatId}", normalizedCommand, chatId);
                    await _telegramService.SendMessageAsync(chatId,
                        "Ukendt kommando. Skriv /hjælp for at se muligheder.", ct);
                    break;
            }
        }

        private async Task SendHelpAsync(long chatId, CancellationToken ct)
        {
            //  Use verbatim string or string.Join for multiline help text — easier to maintain
            var helpText = string.Join("\n",
                "Tilgængelige kommandoer:",
                "/on – Tænd systemet",
                "/off – Sluk systemet",
                "/status – Systemstatus",
                "/hjælp – Vis kommandoer");

            await _telegramService.SendMessageAsync(chatId, helpText, ct);
        }

        private async Task CallBackendAsync(long chatId, string url, CancellationToken ct)
        {
            _logger.LogInformation("Calling backend URL: {Url} for chat {ChatId}", url, chatId);

            try
            {
                var response = await _httpClient.PostAsync(url, null, ct);

                if (!response.IsSuccessStatusCode)
                {
                    // Log the actual status code so failures are diagnosable
                    _logger.LogWarning("Backend returned {StatusCode} for URL {Url}", (int)response.StatusCode, url);
                    await _telegramService.SendMessageAsync(chatId,
                        $"Fejl: Backend returnerede statuskode {(int)response.StatusCode}.", ct);
                    return;
                }

                // Deserialize into JsonDocument instead of object
                // ReadFromJsonAsync<object> deserializes into a JsonElement anyway — be explicit
                using var doc = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonDocument>(cancellationToken: ct);

                if (doc == null)
                {
                    await _telegramService.SendMessageAsync(chatId, "Backend svarede uden indhold.", ct);
                    return;
                }

                //  Serialize with indented options for readable Telegram output
                var prettyJson = System.Text.Json.JsonSerializer.Serialize(
                    doc.RootElement,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                await _telegramService.SendMessageAsync(chatId,
                    $"Backend svar:\n```json\n{prettyJson}\n```", ct);
            }
            //  Catch specific exceptions instead of bare catch {}
            // Bare catch swallows everything including cancellation — very dangerous
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Request to backend {Url} was cancelled for chat {ChatId}", url, chatId);
                throw; // Let cancellation propagate — do not swallow it
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error contacting backend {Url} for chat {ChatId}", url, chatId);
                await _telegramService.SendMessageAsync(chatId,
                    "Fejl: Kunne ikke kontakte backend.", ct);
            }
            catch (Exception ex)
            {
                //  Log unexpected exceptions with full context before sending user feedback
                _logger.LogError(ex, "Unexpected error calling backend {Url} for chat {ChatId}", url, chatId);
                await _telegramService.SendMessageAsync(chatId,
                    "Fejl: En uventet fejl opstod.", ct);
            }
        }
    }
}