namespace Rest_SikkerApi.interfaces
{
    //  Extract interface ITelegramCommandHandler for testability and DI abstraction
    public interface ITelegramCommandHandler
    {
        Task HandleCommandAsync(long chatId, string command, CancellationToken ct = default);
    }
}
