namespace Rest_SikkerApi.models
{
    public class User
    {
        public string OwnerUid { get; set; } = string.Empty;
        public string? TelegramChatId { get; set; }
        public int ReportFrequency { get; set; } = 7;
        public bool ReportEnabled { get; set; } = true; // Default to enabled
    }
}
