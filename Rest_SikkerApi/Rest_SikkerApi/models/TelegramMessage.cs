namespace Rest_SikkerApi.models
{
    public class TelegramMessage
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; } = "";
        public DateTime ReceivedAt { get; set; }
    }
}
