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
    public class TelegramService
    {
        /// <summary>Default Telegram bot token. Override via dependency injection if needed.</summary>
        private readonly string _botToken = "8768336190:AAEcRuOPUmVPIfsKCAXTkSXfzjPHzsKe3vE";

        /// <summary>The Telegram chat ID where all messages are sent.</summary>
        private readonly string _chatId;

        /// <summary>HTTP client for making requests to Telegram's API.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialize the Telegram service with bot credentials.
        /// </summary>
        /// <param name="botToken">Telegram Bot API token. If empty/null, uses default hardcoded token.</param>
        /// <param name="chatId">Target Telegram chat ID. Cannot be null.</param>
        /// <param name="httpClient">Optional HTTP client for unit testing and custom configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if chatId is null or empty.</exception>
        public TelegramService(string botToken, string chatId, HttpClient? httpClient = null)
        {
            if (!string.IsNullOrWhiteSpace(botToken))
            {
                _botToken = botToken;
            }

            _chatId = chatId ?? throw new ArgumentNullException(nameof(chatId));
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Send a simple text message to the configured Telegram chat.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <returns>Task that completes when the message is sent.</returns>
        public virtual async Task SendMessageAsync(string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", _chatId),
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
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL must be provided.", nameof(imageUrl));

            var message = string.IsNullOrWhiteSpace(description)
                ? $"Nyt billede modtaget. Se det her: {imageUrl}"
                : $"Nyt billede modtaget: {description}\nSe det her: {imageUrl}";

            await SendMessageAsync(message);
        }
    }
}
