using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Rest_SikkerApi.Services;
using Xunit;

namespace TestAPI
{
    public class TelegramServiceTests
    {
        [Fact]
        public void Constructor_ThrowsInvalidOperationException_WhenBotTokenIsMissing()
        {
            // Arrange
            var config = new ConfigurationBuilder().Build();
            var httpClient = new HttpClient();
            var logger = NullLogger<TelegramService>.Instance;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                new TelegramService(config, httpClient, logger));
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsArgumentException_WhenChatIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SendMessageAsync(0, "Test message"));
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsArgumentException_WhenMessageIsEmpty()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SendMessageAsync(123456, ""));
        }

        [Fact]
        public async Task SendMessageAsync_SendsCorrectHttpRequest_WhenInputIsValid()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            var handler = new FakeHttpMessageHandler(request =>
            {
                capturedRequest = request;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var service = CreateService(handler);

            // Act
            await service.SendMessageAsync(123456, "Hello Telegram");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest.Method);
            Assert.Equal(
                "https://api.telegram.org/botTEST_TOKEN/sendMessage",
                capturedRequest.RequestUri!.ToString());

            var json = await capturedRequest.Content!.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);

            Assert.Equal(123456, document.RootElement.GetProperty("chat_id").GetInt64());
            Assert.Equal("Hello Telegram", document.RootElement.GetProperty("text").GetString());
            Assert.Equal("Markdown", document.RootElement.GetProperty("parse_mode").GetString());
        }

        [Fact]
        public async Task SendMessageAsync_ThrowsHttpRequestException_WhenTelegramApiReturnsError()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.BadRequest));

            var service = CreateService(handler);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                service.SendMessageAsync(123456, "Test message"));
        }

        private static TelegramService CreateService(HttpMessageHandler? handler = null)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Telegram:BotToken", "TEST_TOKEN" }
                })
                .Build();

            var httpClient = new HttpClient(handler ?? new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK)));

            var logger = NullLogger<TelegramService>.Instance;

            return new TelegramService(config, httpClient, logger);
        }

        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
            {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_handlerFunc(request));
            }
        }
    }
}