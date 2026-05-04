namespace Rest_SikkerApi.models
{
    public class User
    {
        public int Id { get; set; }
        public string FirebaseId { get; set; } = string.Empty;
        public string? ChatId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}