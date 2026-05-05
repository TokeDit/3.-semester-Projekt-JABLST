using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Rest_SikkerApi.interfaces;
using Xunit;

namespace TestAPI
{
    public class TelegramControllerTests
    {
        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenUpdateIsNotJsonObject()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            using var document = JsonDocument.Parse("\"not an object\"");
            var update = document.RootElement.Clone();

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(commandHandler.WasCalled);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenMessagePropertyIsMissing()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "update_id": 123
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(commandHandler.WasCalled);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenChatPropertyIsMissing()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "text": "/start"
                }
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(commandHandler.WasCalled);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenChatIdIsMissing()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "chat": {},
                    "text": "/start"
                }
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.False(commandHandler.WasCalled);
        }

        [Fact]
        public async Task ReceiveUpdate_CallsCommandHandler_WhenUpdateIsValid()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "chat": {
                        "id": 123456
                    },
                    "text": "/start"
                }
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.True(commandHandler.WasCalled);
            Assert.Equal(123456, commandHandler.ReceivedChatId);
            Assert.Equal("/start", commandHandler.ReceivedText);
        }

        [Fact]
        public async Task ReceiveUpdate_UsesEmptyString_WhenTextIsMissing()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "chat": {
                        "id": 123456
                    }
                }
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.True(commandHandler.WasCalled);
            Assert.Equal(123456, commandHandler.ReceivedChatId);
            Assert.Equal("", commandHandler.ReceivedText);
        }

        [Fact]
        public async Task ReceiveUpdate_ReturnsOk_WhenCommandHandlerThrowsHttpRequestException()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler
            {
                ThrowHttpRequestException = true
            };

            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "chat": {
                        "id": 123456
                    },
                    "text": "/start"
                }
            }
            """);

            // Act
            var result = await controller.ReceiveUpdate(update, CancellationToken.None);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.True(commandHandler.WasCalled);
        }

        [Fact]
        public async Task GetStatus_ReturnsLatestTelegramMessage()
        {
            // Arrange
            var commandHandler = new FakeTelegramCommandHandler();
            var controller = CreateController(commandHandler);

            var update = CreateJsonElement("""
            {
                "message": {
                    "chat": {
                        "id": 987654
                    },
                    "text": "hello"
                }
            }
            """);

            await controller.ReceiveUpdate(update, CancellationToken.None);

            // Act
            var result = controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var json = JsonSerializer.Serialize(okResult.Value);
            using var document = JsonDocument.Parse(json);

            Assert.Equal(987654, document.RootElement.GetProperty("lastChatId").GetInt64());
            Assert.Equal("hello", document.RootElement.GetProperty("lastMessage").GetString());
            Assert.False(string.IsNullOrWhiteSpace(
                document.RootElement.GetProperty("lastMessageTime").GetString()));
        }

        private static TelegramController CreateController(
            FakeTelegramCommandHandler commandHandler)
        {
            var telegramService = new FakeTelegramService();

            return new TelegramController(
                telegramService,
                commandHandler,
                NullLogger<TelegramController>.Instance);
        }

        private static JsonElement CreateJsonElement(string json)
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.Clone();
        }

        private class FakeTelegramService : ITelegramService
        {
            public Task SendMessageAsync(
                long chatId,
                string message,
                CancellationToken ct = default)
            {
                return Task.CompletedTask;
            }
        }

        private class FakeTelegramCommandHandler : ITelegramCommandHandler
        {
            public bool WasCalled { get; private set; }
            public long ReceivedChatId { get; private set; }
            public string? ReceivedText { get; private set; }
            public bool ThrowHttpRequestException { get; set; }

            public Task HandleCommandAsync(
                long chatId,
                string text,
                CancellationToken ct = default)
            {
                WasCalled = true;
                ReceivedChatId = chatId;
                ReceivedText = text;

                if (ThrowHttpRequestException)
                {
                    throw new HttpRequestException("Telegram API failed");
                }

                return Task.CompletedTask;
            }
        }
    }
}