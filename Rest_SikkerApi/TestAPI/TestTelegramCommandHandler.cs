using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;

namespace TestAPI
{
    public class TestTelegramCommandHandlerClass
    {
        private static TelegramCommandHandler CreateHandler(
            Mock<ITelegramService>? telegramMock = null,
            Mock<ISikkerRepo>? repoMock = null,
            string authorizedChatIds = "",
            string backendBaseUrl = "http://backend")
        {
            telegramMock ??= new Mock<ITelegramService>();
            repoMock ??= new Mock<ISikkerRepo>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Backend:BaseUrl"]).Returns(backendBaseUrl);
            configMock.Setup(c => c["Telegram:AuthorizedChatIds"]).Returns(authorizedChatIds);

            return new TelegramCommandHandler(
                telegramMock.Object,
                new System.Net.Http.HttpClient(),
                Mock.Of<ILogger<TelegramCommandHandler>>(),
                configMock.Object,
                repoMock.Object);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsWelcome_ForStartCommand()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(100L, "/start");

            telegramMock.Verify(
                t => t.SendMessageAsync(100L, It.Is<string>(s => s.Contains("Welcome")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsHelp_ForHelpCommand()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(100L, "/help");

            telegramMock.Verify(
                t => t.SendMessageAsync(100L, It.Is<string>(s => s.Contains("/on")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsHelp_ForDanishHelpCommand()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(100L, "/hjælp");

            telegramMock.Verify(
                t => t.SendMessageAsync(100L, It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsTime_ForTimeCommand()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(100L, "/time");

            telegramMock.Verify(
                t => t.SendMessageAsync(100L, It.Is<string>(s => s.Contains("tid")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsGreeting_ForHiMessage()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(200L, "hej");

            telegramMock.Verify(
                t => t.SendMessageAsync(200L, It.Is<string>(s => s.Contains("Hej")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsUnknown_ForShortUnknownCommand()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock);

            await handler.HandleCommandAsync(200L, "xyz");

            telegramMock.Verify(
                t => t.SendMessageAsync(200L, It.Is<string>(s => s.Contains("Ukendt")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_LinksFirebaseId_WhenMessageLongerThan20Chars()
        {
            var telegramMock = new Mock<ITelegramService>();
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.UpdateUserChatIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

            var handler = CreateHandler(telegramMock, repoMock);
            var longFirebaseId = "firebase-uid-1234567890abc";

            await handler.HandleCommandAsync(300L, longFirebaseId);

            repoMock.Verify(r => r.UpdateUserChatIdAsync(longFirebaseId, "300", It.IsAny<CancellationToken>()), Times.Once);
            telegramMock.Verify(
                t => t.SendMessageAsync(300L, It.Is<string>(s => s.Contains("knyttet")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_BlocksUnauthorizedChat()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock, authorizedChatIds: "111,222");

            await handler.HandleCommandAsync(999L, "/start");

            telegramMock.Verify(
                t => t.SendMessageAsync(999L, It.Is<string>(s => s.Contains("Unauthorized")), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleCommandAsync_AllowsAuthorizedChat()
        {
            var telegramMock = new Mock<ITelegramService>();
            var handler = CreateHandler(telegramMock, authorizedChatIds: "111,222");

            await handler.HandleCommandAsync(111L, "/start");

            telegramMock.Verify(
                t => t.SendMessageAsync(111L, It.Is<string>(s => s.Contains("Welcome")), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
