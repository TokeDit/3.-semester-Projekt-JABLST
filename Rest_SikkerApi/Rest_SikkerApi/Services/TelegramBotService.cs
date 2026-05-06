using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rest_SikkerApi.Services
{
    /// <summary>
    /// TelegramService handles all communication with the Telegram Bot API.
    /// It sends text messages and photos to a configured Telegram chat.
    /// </summary>
    public class TelegramBotService
    {
        /// <summary>Default Telegram bot token. Override via dependency injection if needed.</summary>
        private readonly string _botToken = "8768336190:AAEcRuOPUmVPIfsKCAXTkSXfzjPHzsKe3vE";

        pirvate readonly ISikkerRepo _repo;

        /// <summary>HTTP client for making requests to Telegram's API.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialize the Telegram service with bot credentials.
        /// </summary>
        /// <param name="botToken">Telegram Bot API token. If empty/null, uses default hardcoded token.</param>
        /// <param name="telegramChatId">Target Telegram chat ID. Cannot be null.</param>
        /// <param name="httpClient">Optional HTTP client for unit testing and custom configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if telegramChatId is null or empty.</exception>
        public TelegramBotService(string botToken, string telegramChatId, HttpClient? httpClient = null)
        {
            if (!string.IsNullOrWhiteSpace(botToken))
            {
                _botToken = botToken;
            }

            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Send a simple text message to the configured Telegram chat.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <returns>Task that completes when the message is sent.</returns>
        public virtual async Task SendMessageAsync(string message)
        {
            await SendMessageAsync(message, _telegramChatId);
        }

        /// <summary>
        /// Send a text message to a specific Telegram chat.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="telegramChatId">Target Telegram chat ID.</param>
        /// <returns>Task that completes when the message is sent.</returns>
        public virtual async Task SendMessageAsync(string message, string telegramChatId)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message text must be provided.", nameof(message));

            if (string.IsNullOrWhiteSpace(telegramChatId))
                throw new ArgumentException("Telegram Chat ID must be provided.", nameof(telegramChatId));

            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", telegramChatId),
                new KeyValuePair<string, string>("text", message)
            });

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Send a text message with a clickable link to the dashboard.
        /// </summary>
        /// <param name="imageUrl">The dashboard URL or any link to include in the message.</param>
        /// <param name="description">Optional image/event description (e.g., "Camera 1 - Motion detected").</param>
        /// <returns>Task that completes when the message is sent.</returns>
        public virtual async Task SendImageLinkAsync(string imageUrl, string description)
        {
            await SendImageLinkAsync(imageUrl, description, _telegramChatId);
        }

        public virtual async Task SendImageLinkAsync(string imageUrl, string description, string ownerUid)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL must be provided.", nameof(imageUrl));


            var user = await _repo.GetUserByFirebaseIdAsync(string ownerUid);
            if (user == null || string.IsNullOrWhiteSpace(user.TelegramChatId))
                throw new InvalidOperationExecption("No Telegram chat ID registered for this user.")

            var message = string.IsNullOrWhiteSpace(description)
                ? $"Nyt billede modtaget. Se det her: {imageUrl}"
                : $"Nyt billede modtaget: {description}\nSe det her: {imageUrl}";

            await SendMessageAsync(message, telegramChatId);
        }
    }
}
