using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

public class TelegramService
{
    private readonly string _botToken = "8768336190:AAEcRuOPUmVPIfsKCAXTkSXfzjPHzsKe3vE";
    private readonly string _chatId;
    private readonly HttpClient _httpClient;

    public TelegramService(string botToken, string chatId)
    {
        _botToken = botToken;
        _chatId = chatId;
        _httpClient = new HttpClient();
    }

    public async Task SendMessageAsync(string message)
    {
        var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("chat_id", _chatId),
            new KeyValuePair<string, string>("text", message)
        });

        await _httpClient.PostAsync(url, content);
    }

    public async Task SendPhotoAsync(byte[] photoBytes, string caption)
    {
        var url = $"https://api.telegram.org/bot{_botToken}/sendPhoto";

        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(photoBytes), "photo", "motion.jpg");
        content.Add(new StringContent(_chatId), "chat_id");
        content.Add(new StringContent(caption), "caption");

        await _httpClient.PostAsync(url, content);
    }
}