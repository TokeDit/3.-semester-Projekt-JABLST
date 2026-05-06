using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Rest_SikkerApi.Repositories; // adjust namespace to match your ISikkerRepo location

namespace Rest_SikkerApi.Services
{
    /// <summary>
    /// TelegramBotService handles all communication with the Telegram Bot API.
    /// It sends text messages and image links to a configured Telegram chat.
    /// </summary>
    public class TelegramBotService
    {
        /// <summary>Default Telegram bot token. Override via dependency injection if needed.</summary>
        private readonly string _botToken = "8768336190:AAEcRuOPUmVPIfsKCAXTkSXfzjPHzsKe3vE";

        /// <summary>Repository for looking up user chat IDs at send time.</summary>
        private readonly ISikkerRepo _repo;

        /// <summary>HTTP client for making requests to Telegram's API.</summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialize the Telegram service with bot credentials and a user repository.
        /// </summary>
        /// <param name="botToken">Telegram Bot API token. If empty/null, uses default hardcoded token.</param>
        /// <param name="repo">Repository used to look up the user's registered Telegram chat ID.</param>
        /// <param name="httpClient">Optional HTTP client for unit testing and custom configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if repo is null.</exception>
        public TelegramBotService(string botToken, ISikkerRepo repo, HttpClient? httpClient = null)
        {
            if (!string.IsNullOrWhiteSpace(botToken))
                _botToken = botToken;

            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Send a text message to a specific Telegram chat.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="telegramChatId">Target Telegram chat ID.</param>
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
        /// Send an image link message to the Telegram chat registered for the given user.
        /// Looks up the user's chat ID dynamically so it always uses the latest registered value.
        /// </summary>
        /// <param name="imageUrl">The URL to include in the message.</param>
        /// <param name="description">Optional description of the image or event.</param>
        /// <param name="ownerUid">Firebase UID used to look up the user's registered chat ID.</param>
        /// <exception cref="InvalidOperationException">Thrown if the user has no registered Telegram chat ID.</exception>
        public virtual async Task SendImageLinkAsync(string imageUrl, string description, string ownerUid)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL must be provided.", nameof(imageUrl));

            // Look up the user's current chat ID from the database rather than using a stale constructor value
            var user = await _repo.GetUserByFirebaseIdAsync(ownerUid);
            if (user == null || string.IsNullOrWhiteSpace(user.TelegramChatId))
                throw new InvalidOperationException("No Telegram chat ID registered for this user.");

            // Build the message, omitting description prefix if none was provided
            var message = string.IsNullOrWhiteSpace(description)
                ? $"Nyt billede modtaget. Se det her: {imageUrl}"
                : $"Nyt billede modtaget: {description}\nSe det her: {imageUrl}";

            await SendMessageAsync(message, user.TelegramChatId);
        }
    }
}
