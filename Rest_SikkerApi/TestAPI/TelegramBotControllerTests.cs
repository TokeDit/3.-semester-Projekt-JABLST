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
        [Fact]
        public async Task SendMotionAlertMessage_ReturnsBadRequest_WhenDescriptionIsEmpty()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var result = await controller.SendMotionAlertMessage(string.Empty);

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendMessageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertMessage_CallsSendMessageAsync_WhenDescriptionIsValid()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            serviceMock.Setup(s => s.SendMessageAsync("Motion detected! Front door"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();
            var repoMock = new Mock<ISikkerRepo>();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var result = await controller.SendMotionAlertMessage("Front door");

            Assert.IsType<OkObjectResult>(result);
            serviceMock.Verify();
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenRequestIsNull()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.SendMotionAlertLink(null!);

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenImageUrlIsEmpty()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.SendMotionAlertLink(new TelegramLinkRequest { ImageUrl = string.Empty, Description = "Test" });

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_CallsSendImageLinkAsync_WhenRequestIsValid()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            serviceMock.Setup(s => s.SendImageLinkAsync("https://dashboard.example", "Door alert"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();
            var repoMock = new Mock<ISikkerRepo>();

            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);
            var request = new TelegramLinkRequest
            {
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
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();
            var controller = new TelegramBotController(serviceMock.Object, repoMock.Object);

            var result = await controller.HandleWebhook(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task HandleWebhook_RegistersChatId_WhenFirebaseIdIsValid()
        {
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();

            repoMock.Setup(r => r.GetUserByFirebaseIdAsync("valid-firebase-id-very-long"))
                    .ReturnsAsync(new User { OwnerUid = "valid-firebase-id-very-long" });
            repoMock.Setup(r => r.UpdateUserChatIdAsync("valid-firebase-id-very-long", "123456789", default(CancellationToken)))
                    .ReturnsAsync(true).Verifiable();



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
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByFirebaseIdAsync("invalid-firebase-id-very-long", It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

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
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByChatIdAsync("123456789"))
                    .ReturnsAsync(new User { OwnerUid = "some-Uid", TelegramChatId = "123456789" });

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
            var serviceMock = new Mock<TelegramBotService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByChatIdAsync("123456789"))
                    .ReturnsAsync((User?)null);

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
