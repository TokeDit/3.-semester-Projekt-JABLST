using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Rest_SikkerApi.Services;
using Xunit;

namespace TestAPI
{
    public class TelegramServiceTests
    {
        private static HttpClient CreateHttpClient(Mock<HttpMessageHandler> handlerMock)
        {
            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsWhenChatIdIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TelegramService("bot-token", null!, new HttpClient()));
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
                .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var httpClient = CreateHttpClient(handlerMock);
            var service = new TelegramService("bot-token", "123", httpClient);

            await service.SendMessageAsync("Hello world");

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
                .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var httpClient = CreateHttpClient(handlerMock);
            var service = new TelegramService("bot-token", "123", httpClient);

            await service.SendImageLinkAsync("https://dashboard.example", string.Empty);

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
                .Callback<HttpRequestMessage, CancellationToken>((request, cancellationToken) => capturedRequest = request)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK))
                .Verifiable();

            var httpClient = CreateHttpClient(handlerMock);
            var service = new TelegramService("bot-token", "123", httpClient);

            await service.SendImageLinkAsync("https://dashboard.example", "Front door alert");

            Assert.NotNull(capturedRequest);
            var decodedBody = WebUtility.UrlDecode(await capturedRequest!.Content!.ReadAsStringAsync());
            Assert.Contains("text=Nyt billede modtaget: Front door alert\nSe det her: https://dashboard.example", decodedBody);
        }

        [Fact]
        public async Task SendImageLinkAsync_ThrowsWhenImageUrlIsEmpty()
        {
            var httpClient = new HttpClient();
            var service = new TelegramService("bot-token", "123", httpClient);

            await Assert.ThrowsAsync<ArgumentException>(async () => await service.SendImageLinkAsync(string.Empty, "Description"));
        }

    }
}
