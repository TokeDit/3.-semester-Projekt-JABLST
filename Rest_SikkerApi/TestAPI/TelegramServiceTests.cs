using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;
using Xunit;

namespace TestAPI
{
    public class TelegramServiceTests
    {
        // Creates an HttpClient backed by a mock handler that captures outgoing requests
        private static HttpClient CreateHttpClient(Mock<HttpMessageHandler> handlerMock)
        {
            return new HttpClient(handlerMock.Object);
        }

        // Sets up a strict mock handler that returns 200 OK and captures the request
        private static Mock<HttpMessageHandler> CreateHandlerMock(out HttpRequestMessage? captured)
        {
            HttpRequestMessage? cap = null;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => cap = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();
            captured = cap;
            return handlerMock;
        }

        // Creates a TelegramBotService with a repo mock that returns a user with the given chat ID
        private static TelegramBotService CreateService(string chatId, HttpClient httpClient, out Mock<ISikkerRepo> repoMock)
        {
            var mock = new Mock<ISikkerRepo>();
            // Return a user with the specified chat ID for any Firebase ID lookup
            mock.Setup(r => r.GetUserByFirebaseIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { OwnerUid = "test-uid", TelegramChatId = chatId });
            repoMock = mock;
            return new TelegramBotService("bot-token", mock.Object, httpClient);
        }

        [Fact]
        public void Constructor_ThrowsWhenRepoIsNull()
        {
            // Repo is now required — passing null should throw
            Assert.Throws<ArgumentNullException>(() => new TelegramBotService("bot-token", (ISikkerRepo)null!, new HttpClient()));
        }

        [Fact]
        public async Task SendMessageAsync_SendsExpectedTelegramApiRequest()
        {
            HttpRequestMessage? capturedRequest = null;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var service = CreateService("123", CreateHttpClient(handlerMock), out _);

            // SendMessageAsync now requires an explicit chat ID
            await service.SendMessageAsync("Hello world", "123");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

            Assert.NotNull(capturedRequest);
            Assert.Equal(HttpMethod.Post, capturedRequest!.Method);
            Assert.Equal("https://api.telegram.org/botbot-token/sendMessage", capturedRequest.RequestUri?.ToString());

            var body = await capturedRequest.Content!.ReadAsStringAsync();
            var decodedBody = WebUtility.UrlDecode(body);
            Assert.Contains("chat_id=123", decodedBody);
            Assert.Contains("text=Hello world", decodedBody);
        }

        [Fact]
        public async Task SendImageLinkAsync_WithoutDescription_FormatsMessageCorrectly()
        {
            HttpRequestMessage? capturedRequest = null;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var service = CreateService("123", CreateHttpClient(handlerMock), out _);

            // Pass ownerUid — the service will look up the chat ID from the repo mock
            await service.SendImageLinkAsync("https://dashboard.example", string.Empty, "test-uid");

            Assert.NotNull(capturedRequest);
            var decodedBody = WebUtility.UrlDecode(await capturedRequest!.Content!.ReadAsStringAsync());
            Assert.Contains("text=Nyt billede modtaget. Se det her: https://dashboard.example", decodedBody);
        }

        [Fact]
        public async Task SendImageLinkAsync_WithDescription_FormatsMessageCorrectly()
        {
            HttpRequestMessage? capturedRequest = null;
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var service = CreateService("123", CreateHttpClient(handlerMock), out _);

            // Pass ownerUid — the service will look up the chat ID from the repo mock
            await service.SendImageLinkAsync("https://dashboard.example", "Front door alert", "test-uid");

            Assert.NotNull(capturedRequest);
            var decodedBody = WebUtility.UrlDecode(await capturedRequest!.Content!.ReadAsStringAsync());
            Assert.Contains("text=Nyt billede modtaget: Front door alert\nSe det her: https://dashboard.example", decodedBody);
        }

        [Fact]
        public async Task SendImageLinkAsync_ThrowsWhenImageUrlIsEmpty()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var service = new TelegramBotService("bot-token", repoMock.Object, new HttpClient());

            // Empty image URL should throw regardless of ownerUid
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.SendImageLinkAsync(string.Empty, "Description", "test-uid"));
        }
    }
}
