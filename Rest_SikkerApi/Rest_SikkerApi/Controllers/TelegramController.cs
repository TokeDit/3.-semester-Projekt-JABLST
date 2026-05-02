using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.interfaces;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("telegram")]
    public class TelegramController : ControllerBase
    {
        // Depend on ITelegramService abstraction, not concrete TelegramService
        private readonly ITelegramService _telegramService;

        // COMMIT  Replace static in-memory state with a dedicated state class to avoid
        // race conditions and make state testable and replaceable (e.g. with a DB later)
        private static long _lastChatId;
        private static string _lastMessage = "";
        private static DateTime _lastMessageTime;

        //  Inject ILogger for structured logging in controller actions
        private readonly ILogger<TelegramController> _logger;
    
     // COMMIT 1: Inject ITelegramService instead of TelegramService
        // COMMIT 3: Inject ILogger
        public TelegramController(ITelegramService telegramService, ILogger<TelegramController> logger)
        {
            _telegramService = telegramService;
            _logger = logger;
        }
    }
}
