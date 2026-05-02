namespace Rest_SikkerApi.interfaces
{
    public interface ITelegramService
    {
        Task SendMessageAsync(long chatId, string message, CancellationToken ct = default);
    }
}
