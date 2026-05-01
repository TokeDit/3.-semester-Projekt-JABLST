using System;
using System.IO;
using System.Threading.Tasks;

public class TelegramBotController
{
    private readonly TelegramService _telegramService;

    public TelegramBotController(TelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    /// <summary>
    /// Sends a motion alert message to the configured Telegram chat.
    /// </summary>
    /// <param name="description">Description of the event or camera location.</param>
    public async Task SendMotionAlertMessage(string description)
    {
        string message = $"Motion detected! {description}";
        await _telegramService.SendMessage(message);
        Console.WriteLine("Motion alert message sent to Telegram.");
    }

    /// <summary>
    /// Sends a motion alert including a photo to the Telegram chat.
    /// </summary>
    /// <param name="imagePath">Path to the captured image file.</param>
    /// <param name="description">Description of the event or camera location.</param>
    public async Task SendMotionAlertWithPhoto(string imagePath, string description)
    {
        if (!File.Exists(imagePath))
        {
            Console.WriteLine("Image file not found.");
            return;
        }

        string caption = $"Motion detected! {description}";
        await _telegramService.SendPhoto(imagePath, caption);
        Console.WriteLine("Motion alert with photo sent to Telegram.");
    }
}