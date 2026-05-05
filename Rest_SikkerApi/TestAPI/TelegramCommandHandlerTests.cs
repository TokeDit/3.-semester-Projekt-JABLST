using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.Services;
using Xunit;

namespace TestAPI
{
    public class TelegramCommandHandlerTests
    {
        [Fact]
        public void Constructor_ThrowsInvalidOperationException_WhenBackendBaseUrlIsMissing()
        {
            // Arrange
            var telegramService = new FakeTelegramService();
            var httpClient = new HttpClient();
            var logger = NullLogger<TelegramCommandHandler>.Instance;
            var config = new ConfigurationBuilder().Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                new TelegramCommandHandler(telegramService, httpClient, logger, config));
        }

        [Theory]
        [InlineData("/help")]
        [InlineData("/hjælp")]
        public async Task HandleCommandAsync_SendsHelpMessage_WhenCommandIsHelp(string command)
        {
            // Arrange
            var telegramService = new FakeTelegramService();
            var handler = CreateHandler(telegramService);

            // Act
            await handler.HandleCommandAsync(123, command);

            // Assert
            Assert.Contains("Available Commands", telegramService.LastMessage);
            Assert.Contains("/on", telegramService.LastMessage);
            Assert.Contains("/off", telegramService.LastMessage);
            Assert.Contains("/status", telegramService.LastMessage);
        }

        [Theory]
        [InlineData("hi")]
        [InlineData("hello")]
        [InlineData("hej")]
        [InlineData("hey")]
        [InlineData("godmorgen")]
        [InlineData("godaften")]
        public async Task HandleCommandAsync_SendsGreetingMessage_WhenCommandIsGreeting(string command)
        {
            // Arrange
            var telegramService = new FakeTelegramService();
            var handler = CreateHandler(telegramService);

            // Act
            await handler.HandleCommandAsync(123, command);

            // Assert
            Assert.Equal("Hej! Hvordan kan jeg hjælpe dig i dag?", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsUnknownCommandMessage_WhenCommandIsUnknown()
        {
            // Arrange
            var telegramService = new FakeTelegramService();
            var handler = CreateHandler(telegramService);

            // Act
            await handler.HandleCommandAsync(123, "/doesnotexist");

            // Assert
            Assert.Equal("Ukendt kommando. Skriv /hjælp for at se muligheder.", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsServerTime_WhenCommandIsTime()
        {
            // Arrange
            var telegramService = new FakeTelegramService();
            var handler = CreateHandler(telegramService);

            // Act
            await handler.HandleCommandAsync(123, "/time");

            // Assert
            Assert.StartsWith("Server tid:", telegramService.LastMessage);
        }

        [Theory]
        [InlineData("/on", "http://localhost:5000/Sikker/on")]
        [InlineData("/off", "http://localhost:5000/Sikker/off")]
        public async Task HandleCommandAsync_CallsBackendPost_WhenCommandIsOnOrOff(
            string command,
            string expectedUrl)
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(request =>
            {
                capturedRequest = request;

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "status": "ok" }""")
                };
            }));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, command);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest.Method);
            Assert.Equal(expectedUrl, capturedRequest.RequestUri!.ToString());
            Assert.Contains("Backend svar:", telegramService.LastMessage);
            Assert.Contains("status", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_CallsBackendGet_WhenCommandIsStatus()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(request =>
            {
                capturedRequest = request;

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{ "status": "online" }""")
                };
            }));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, "/status");

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Get, capturedRequest.Method);
            Assert.Equal("http://localhost:5000/Sikker/status", capturedRequest.RequestUri!.ToString());
            Assert.Contains("Backend svar:", telegramService.LastMessage);
            Assert.Contains("online", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsErrorMessage_WhenBackendReturnsError()
        {
            // Arrange
            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.InternalServerError)));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, "/on");

            // Assert
            Assert.Equal("Fejl: Backend returnerede statuskode 500.", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsContactBackendError_WhenHttpRequestExceptionOccurs()
        {
            // Arrange
            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
                throw new HttpRequestException("Backend unavailable")));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, "/status");

            // Assert
            Assert.Equal("Fejl: Kunne ikke kontakte backend.", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsPong_WhenPingSucceeds()
        {
            // Arrange
            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK)));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, "/ping");

            // Assert
            Assert.StartsWith("Pong! Response time:", telegramService.LastMessage);
        }

        [Fact]
        public async Task HandleCommandAsync_SendsPingFailed_WhenPingFails()
        {
            // Arrange
            var telegramService = new FakeTelegramService();

            var httpClient = new HttpClient(new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));

            var handler = CreateHandler(telegramService, httpClient);

            // Act
            await handler.HandleCommandAsync(123, "/ping");

            // Assert
            Assert.Equal("Ping failed — backend not responding.", telegramService.LastMessage);
        }

        private static TelegramCommandHandler CreateHandler(
            FakeTelegramService telegramService,
            HttpClient? httpClient = null)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Backend:BaseUrl", "http://localhost:5000" }
                })
                .Build();

            return new TelegramCommandHandler(
                telegramService,
                httpClient ?? new HttpClient(new FakeHttpMessageHandler(_ =>
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("""{ "status": "ok" }""")
                    })),
                NullLogger<TelegramCommandHandler>.Instance,
                config);
        }

        private class FakeTelegramService : ITelegramService
        {
            public long LastChatId { get; private set; }
            public string LastMessage { get; private set; } = "";
            public int SendCount { get; private set; }

            public Task SendMessageAsync(
                long chatId,
                string message,
                CancellationToken ct = default)
            {
                LastChatId = chatId;
                LastMessage = message;
                SendCount++;

                return Task.CompletedTask;
            }
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