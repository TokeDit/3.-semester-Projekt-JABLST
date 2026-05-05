namespace Rest_SikkerApi.models
{
    public class User
    {
        public string OwnerUid { get; set; } = string.Empty;
        public string? TelegramChatId { get; set; }
    }
}