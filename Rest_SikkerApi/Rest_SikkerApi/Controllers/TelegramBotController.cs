using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelegramBotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly string _chatId;

        public TelegramBotController(ITelegramBotClient botClient, IConfiguration configuration)
        {
            _botClient = botClient;
            _chatId = configuration["Telegram:ChatId"];
        }

        [HttpPost("receiveImage")]
        public async Task<IActionResult> ReceiveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided");
            }

            using var stream = image.OpenReadStream();
            
            // Create InputFileStream using the file stream
            var inputFile = new InputFileStream(stream, image.FileName);

            // Send the photo using SendPhotoAsync
            await _botClient.SendPhoto(
                _chatId,  // The chat ID where to send the photo
                inputFile,  // The photo file (from the stream)
                "Image with human from Pi"  // Optional caption
            );

            return Ok();
        }
    }
}