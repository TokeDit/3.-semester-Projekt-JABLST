using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace Rest_SikkerApi.Services
{
    //  Background service that sends scheduled reports via Telegram
    public class ReportService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReportService> _logger;

        // Check every hour if any reports need sending
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ReportService(IServiceScopeFactory scopeFactory, ILogger<ReportService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("ReportService started.");

            while (!ct.IsCancellationRequested)
            {
                await SendScheduledReportsAsync(ct);
                await Task.Delay(_checkInterval, ct);
            }
        }

        private async Task SendScheduledReportsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISikkerRepo>();
            var telegramService = scope.ServiceProvider.GetRequiredService<TelegramBotService>();

            var now = DateTime.UtcNow;

            // Only send at 08:00 UTC
            if (now.Hour != 8) return;

            try
            {
                var users = await repo.GetUsersWithReportsEnabledAsync();
                _logger.LogInformation("Sending reports to {Count} users.", users.Count);

                foreach (var user in users)
                { 
                    try
                    {

                        var images = await repo.GetImagesByOwnerUidSinceAsync(user.OwnerUid, user.ReportFrequency);
                        var periodLabel = user.ReportFrequency;

                        // Build report message
                        var report = BuildReport(images, periodLabel.ToString());

                        await telegramService.SendMessageAsync(report, user.TelegramChatId!);

                        _logger.LogInformation("Report sent to user {OwnerUid}", user.OwnerUid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send report to user {OwnerUid}", user.OwnerUid);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendScheduledReportsAsync");
            }
        }

        private string BuildReport(List<Rest_SikkerApi.models.Image> images, string period)
        {
            if (!images.Any())
            {
                return $"📊 *Report — last {period}*\n\nNo events detected.";
            }

            var lines = new System.Text.StringBuilder();
            lines.AppendLine($"📊 *Report — last {period}*");
            lines.AppendLine($"Total events: {images.Count}");
            lines.AppendLine();

            foreach (var img in images.Take(10)) // max 10 events
            {
                var description = string.IsNullOrWhiteSpace(img.Description)
                    ? "Motion detected"
                    : img.Description;
                lines.AppendLine($"• {img.TimeStamp} — {description}");
            }

            if (images.Count > 10)
                lines.AppendLine($"... and {images.Count - 10} more events.");

            return lines.ToString();
        }
    }
}
