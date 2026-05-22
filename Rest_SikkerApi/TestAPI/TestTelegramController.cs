using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Rest_SikkerApi.data;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.models;
using System.Text.Json;

namespace TestAPI
{
    public class TestTelegramControllerClass
    {
        private static DbContextOptions<AppDbContext> InMemoryOptions() =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private static TelegramController CreateController(
            AppDbContext? context = null,
            Mock<ITelegramService>? telegramMock = null,
            Mock<ITelegramCommandHandler>? commandMock = null)
        {
            context ??= new AppDbContext(InMemoryOptions());
            telegramMock ??= new Mock<ITelegramService>();
            commandMock ??= new Mock<ITelegramCommandHandler>();
            return new TelegramController(
                telegramMock.Object,
                commandMock.Object,
                Mock.Of<ILogger<TelegramController>>(),
                context);
        }

        private static JsonElement Parse(string json) =>
            JsonDocument.Parse(json).RootElement;

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenNotJsonObject()
        {
            var controller = CreateController();
            var result = await controller.ReceiveUpdate(Parse("\"just a string\""), default);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenNoMessageProperty()
        {
            var controller = CreateController();
            var result = await controller.ReceiveUpdate(Parse(@"{ ""something"": 1 }"), default);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenMessageHasNoChat()
        {
            var controller = CreateController();
            var result = await controller.ReceiveUpdate(
                Parse(@"{ ""message"": { ""text"": ""hello"" } }"), default);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenChatHasNoId()
        {
            var controller = CreateController();
            var result = await controller.ReceiveUpdate(
                Parse(@"{ ""message"": { ""chat"": { ""name"": ""test"" }, ""text"": ""hi"" } }"), default);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ReceiveUpdate_CallsCommandHandler_WhenValidMessage()
        {
            var commandMock = new Mock<ITelegramCommandHandler>();
            commandMock.Setup(c => c.HandleCommandAsync(123456L, "/start", It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

            var controller = CreateController(commandMock: commandMock);

            var result = await controller.ReceiveUpdate(
                Parse(@"{ ""message"": { ""chat"": { ""id"": 123456 }, ""text"": ""/start"" } }"), default);

            Assert.IsType<OkResult>(result);
            commandMock.Verify();
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenCommandHandlerThrows()
        {
            var commandMock = new Mock<ITelegramCommandHandler>();
            commandMock.Setup(c => c.HandleCommandAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new Exception("handler error"));

            var controller = CreateController(commandMock: commandMock);
            var result = await controller.ReceiveUpdate(
                Parse(@"{ ""message"": { ""chat"": { ""id"": 999 }, ""text"": ""boom"" } }"), default);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetStatus_ReturnsOk_WhenNoMessages()
        {
            var controller = CreateController();
            var result = await controller.GetStatus();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetStatus_ReturnsLastMessage_WhenMessagesExist()
        {
            var context = new AppDbContext(InMemoryOptions());
            context.TelegramMessages.Add(new TelegramMessage
            {
                ChatId = 777,
                Message = "test",
                ReceivedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context: context);
            var result = (OkObjectResult)await controller.GetStatus();
            var value = result.Value!;
            var lastChatId = (long)value.GetType().GetProperty("lastChatId")!.GetValue(value)!;

            Assert.Equal(777L, lastChatId);
        }
    }
}
