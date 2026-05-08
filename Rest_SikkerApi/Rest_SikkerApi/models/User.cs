namespace Rest_SikkerApi.models
{
    public class User
    {
        public string OwnerUid { get; set; } = string.Empty;
        public string? TelegramChatId { get; set; }
        public string ReportFrequency { get; set; } = "Daily"; // Default to "Daily"
        public bool ReportEnabled { get; set; } = true; // Default to enabled
    }
}