using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Rest_SikkerApi.Services;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace TestAPI
{
    public class TestTelegramServiceClass
    {
        private static TelegramService CreateService(Mock<HttpMessageHandler> handlerMock, string botToken = "test-token")
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Telegram:BotToken"]).Returns(botToken);

            var httpClient = new HttpClient(handlerMock.Object);
            return new TelegramService(configMock.Object, httpClient, Mock.Of<ILogger<TelegramService>>());
        }

        private static Mock<HttpMessageHandler> OkHandler()
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
            return handler;
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsArgumentException_WhenChatIdIsZero()
        {
            var service = CreateService(OkHandler());
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SendMessageAsync(0L, "hello"));
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsArgumentException_WhenMessageIsEmpty()
        {
            var service = CreateService(OkHandler());
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SendMessageAsync(123L, string.Empty));
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsArgumentException_WhenMessageIsWhitespace()
        {
            var service = CreateService(OkHandler());
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SendMessageAsync(123L, "   "));
        }

        [Fact]
        public async Task SendMessageAsync_PostsToCorrectUrl()
        {
            HttpRequestMessage? captured = null;
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var service = CreateService(handler, "mytoken");
            await service.SendMessageAsync(12345L, "Hello Telegram");

            Assert.NotNull(captured);
            Assert.Equal(HttpMethod.Post, captured!.Method);
            Assert.Equal(
                "https://api.telegram.org/botmytoken/sendMessage",
                captured.RequestUri?.ToString());
        }

        [Fact]
        public async Task SendMessageAsync_IncludesChatIdAndTextInBody()
        {
            HttpRequestMessage? captured = null;
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => captured = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var service = CreateService(handler);
            await service.SendMessageAsync(99L, "Test message");

            var body = await captured!.Content!.ReadAsStringAsync();
            var doc = JsonDocument.Parse(body);
            Assert.Equal(99, doc.RootElement.GetProperty("chat_id").GetInt64());
            Assert.Equal("Test message", doc.RootElement.GetProperty("text").GetString());
        }

        [Fact]
        public void Constructor_ThrowsInvalidOperation_WhenBotTokenMissing()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Telegram:BotToken"]).Returns((string?)null);

            Assert.Throws<InvalidOperationException>(() =>
                new TelegramService(configMock.Object, new HttpClient(), Mock.Of<ILogger<TelegramService>>()));
        }
    }
}
