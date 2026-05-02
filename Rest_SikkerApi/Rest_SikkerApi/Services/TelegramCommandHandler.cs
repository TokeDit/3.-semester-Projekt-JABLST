using Rest_SikkerApi.interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Rest_SikkerApi.Services
{
    // COMMIT 10: Extract interface ITelegramCommandHandler for testability and DI abstraction
    public interface ITelegramCommandHandler
    {
        Task HandleCommandAsync(long chatId, string command, CancellationToken ct = default);
    }
}

