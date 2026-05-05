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
        private async Task CallBackendGetAsync(long chatId, string url, CancellationToken ct)
        {
            _logger.LogInformation("GET backend URL: {Url} for chat {ChatId}", url, chatId);

            try
            {
                var response = await _httpClient.GetAsync(url, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Backend returned {StatusCode} for URL {Url}", (int)response.StatusCode, url);
                    await _telegramService.SendMessageAsync(chatId,
                        $"Fejl: Backend returnerede statuskode {(int)response.StatusCode}.", ct);
                    return;
                }

                using var doc = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonDocument>(cancellationToken: ct);

                if (doc == null)
                {
                    await _telegramService.SendMessageAsync(chatId, "Backend svarede uden indhold.", ct);
                    return;
                }

                var prettyJson = System.Text.Json.JsonSerializer.Serialize(
                    doc.RootElement,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                await _telegramService.SendMessageAsync(chatId,
                    $"Backend svar:\n```json\n{prettyJson}\n```", ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GET request to {Url} was cancelled", url);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error on GET {Url} for chat {ChatId}", url, chatId);
                await _telegramService.SendMessageAsync(chatId, "Fejl: Kunne ikke kontakte backend.", ct);
            }
        }

        public async Task HandleCommandAsync(long chatId, string command, CancellationToken ct = default)
        {
            var normalizedCommand = command.Trim().ToLower();

            _logger.LogInformation("Handling command '{Command}' for chat {ChatId}", normalizedCommand, chatId);

            switch (normalizedCommand)
            {
                //Start Menu for Telegram Bot
                case "/start":
                    await _telegramService.SendMessageAsync(chatId,
                        "Welcome to Vision Monitor Bot 👋\n\n" +
                        "I can help you control and monitor your system.\n\n" +
                        "Available commands:\n" +
                        "/on – Turn the system on\n" +
                        "/off – Turn the system off\n" +
                        "/status – System status\n" +
                        "/time – Server time\n" +
                        "/help – Show all commands\n\n" +
                        "Type a command to get started.", ct);
                    break;
                // --- HELP ---
                case "/hjælp":
                case "/help":
                    await SendHelpAsync(chatId, ct);
                    break;

                // --- BACKEND COMMANDS ---
                case "/on":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/on", ct);
                    break;

                case "/off":
                    await CallBackendAsync(chatId, $"{_backendBaseUrl}/Sikker/off", ct);
                    break;

                case "/status":
                    await CallBackendGetAsync(chatId, $"{_backendBaseUrl}/Sikker/status", ct);
                    break;
                case "/time":
                    await _telegramService.SendMessageAsync(chatId, $"Server tid: {DateTime.UtcNow:O}", ct);
                    break;
                    case "/ping":
                        await PingAsync(chatId, ct);
                        break;

                // --- GREETINGS ---
                case "hi":
                case "hello":
                case "hej":
                case "hey":
                case "godmorgen":
                case "godaften":
                    await _telegramService.SendMessageAsync(chatId,
                        "Hej! Hvordan kan jeg hjælpe dig i dag?", ct);
                    break;

                // --- UNKNOWN ---
                default:
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
                "Available Commands:",
                "/on – Turn on the system",
                "/off – Turn off the system",
                "/status – System status",
                "/time – Server time",
                "/help – Show commands");
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
        private async Task PingAsync(long chatId, CancellationToken ct)
        {
            var start = DateTime.UtcNow;
            try
            {
                var response = await _httpClient.GetAsync($"{_backendBaseUrl}/Sikker/ping", ct);
                var ms = (DateTime.UtcNow - start).TotalMilliseconds;

                if (response.IsSuccessStatusCode)
                    await _telegramService.SendMessageAsync(chatId,
                        $"Pong! Response time: {ms:F0}ms", ct);
                else
                    await _telegramService.SendMessageAsync(chatId,
                        "Ping failed — backend not responding.", ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Ping request was cancelled for chat {ChatId}", chatId);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ping failed for chat {ChatId}", chatId);
                await _telegramService.SendMessageAsync(chatId,
                    "Ping failed — backend not responding.", ct);
            }
        }
    }
}