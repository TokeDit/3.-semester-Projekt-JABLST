using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rest_SikkerApi.Controllers;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;
using Xunit;
using TelegramMessage = Rest_SikkerApi.models.TelegramMessage;

namespace TestAPI
{
    public class TelegramBotControllerTests
    {
        // Helper that creates a TelegramBotService mock using the updated constructor signature (ISikkerRepo instead of string chatId)
        private static Mock<TelegramBotService> CreateServiceMock(ISikkerRepo repo)
        {
            return new Mock<TelegramBotService>("bot-token", repo, new HttpClient())
            {
                CallBase = false
            };
        }

        [Fact]
        public async Task SendMotionAlertMessage_ReturnsBadRequest_WhenDescriptionIsEmpty()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            // Pass a request with an empty description
            var result = await controller.SendMotionAlertMessage(new TelegramMessageRequest
            {
                OwnerUid = "some-uid",
                Description = string.Empty
            });

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertMessage_CallsSendMessageAsync_WhenDescriptionIsValid()
        {
            var repoMock = new Mock<ISikkerRepo>();

            // Return a user with a registered chat ID so the controller can look it up
            repoMock.Setup(r => r.GetUserByFirebaseIdAsync("valid-uid"))
                    .ReturnsAsync(new User { OwnerUid = "valid-uid", TelegramChatId = "123456789" });

            var serviceMock = CreateServiceMock(repoMock.Object);
            serviceMock.Setup(s => s.SendMessageAsync("Motion detected! Front door", "123456789"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.SendMotionAlertMessage(new TelegramMessageRequest
            {
                OwnerUid = "valid-uid",
                Description = "Front door"
            });

            Assert.IsType<OkObjectResult>(result);
            serviceMock.Verify();
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenRequestIsNull()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.SendMotionAlertLink(null!);

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenImageUrlIsEmpty()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.SendMotionAlertLink(new TelegramLinkRequest
            {
                OwnerUid = "some-uid",
                ImageUrl = string.Empty,
                Description = "Test"
            });

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_CallsSendImageLinkAsync_WhenRequestIsValid()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var serviceMock = CreateServiceMock(repoMock.Object);

            // Expect the call with all three arguments including ownerUid
            serviceMock.Setup(s => s.SendImageLinkAsync("https://dashboard.example", "Door alert", "valid-uid"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramLinkRequest
            {
                OwnerUid = "valid-uid",
                ImageUrl = "https://dashboard.example",
                Description = "Door alert"
            };

            var result = await controller.SendMotionAlertLink(request);

            Assert.IsType<OkObjectResult>(result);
            serviceMock.Verify();
        }

        [Fact]
        public async Task HandleWebhook_ReturnsBadRequest_WhenRequestIsInvalid()
        {
            var repoMock = new Mock<ISikkerRepo>();
            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.HandleWebhook(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task HandleWebhook_RegistersChatId_WhenFirebaseIdIsValid()
        {
            var repoMock = new Mock<ISikkerRepo>();

            // Simulate a successful chat ID registration for a known Firebase ID
            repoMock.Setup(r => r.UpdateUserChatIdAsync("valid-firebase-id-very-long", "123456789", default(CancellationToken)))
                    .ReturnsAsync(true)
                    .Verifiable();

            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramMessage
            {
                ChatId = 123456789,
                Message = "valid-firebase-id-very-long"
            };

            var result = await controller.HandleWebhook(request);

            Assert.IsType<OkObjectResult>(result);
            repoMock.Verify();
        }

        [Fact]
        public async Task HandleWebhook_ReturnsBadRequest_WhenFirebaseIdNotFound()
        {
            var repoMock = new Mock<ISikkerRepo>();

            // Simulate UpdateUserChatIdAsync returning false — no user matched the Firebase ID
            repoMock.Setup(r => r.UpdateUserChatIdAsync("invalid-firebase-id-very-long", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramMessage
            {
                ChatId = 123456789,
                Message = "invalid-firebase-id-very-long"
            };

            var result = await controller.HandleWebhook(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task HandleWebhook_ReturnsOk_WhenChatIdIsRegistered()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByChatIdAsync("123456789"))
                    .ReturnsAsync(new User { OwnerUid = "some-Uid", TelegramChatId = "123456789" });

            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramMessage
            {
                ChatId = 123456789,
                Message = "some message"
            };

            var result = await controller.HandleWebhook(request);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task HandleWebhook_ReturnsBadRequest_WhenChatIdNotRegistered()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByChatIdAsync("123456789"))
                    .ReturnsAsync((User?)null);

            var serviceMock = CreateServiceMock(repoMock.Object);
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramMessage
            {
                ChatId = 123456789,
                Message = "some message"
            };

            var result = await controller.HandleWebhook(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
