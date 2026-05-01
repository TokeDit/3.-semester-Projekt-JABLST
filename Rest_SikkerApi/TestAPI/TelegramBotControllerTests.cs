using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rest_SikkerApi.Controllers;
using Rest_SikkerApi.Services;
using Xunit;

namespace TestAPI
{
    public class TelegramBotControllerTests
    {
        [Fact]
        public async Task SendMotionAlertMessage_ReturnsBadRequest_WhenDescriptionIsEmpty()
        {
            var serviceMock = new Mock<TelegramService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };

            var controller = new TelegramBotController(serviceMock.Object);
            var result = await controller.SendMotionAlertMessage(string.Empty);

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendMessageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertMessage_CallsSendMessageAsync_WhenDescriptionIsValid()
        {
            var serviceMock = new Mock<TelegramService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            serviceMock.Setup(s => s.SendMessageAsync("Motion detected! Front door"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

            var controller = new TelegramBotController(serviceMock.Object);
            var result = await controller.SendMotionAlertMessage("Front door");

            Assert.IsType<OkObjectResult>(result);
            serviceMock.Verify();
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenRequestIsNull()
        {
            var serviceMock = new Mock<TelegramService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var controller = new TelegramBotController(serviceMock.Object);

            var result = await controller.SendMotionAlertLink(null!);

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_ReturnsBadRequest_WhenImageUrlIsEmpty()
        {
            var serviceMock = new Mock<TelegramService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            var controller = new TelegramBotController(serviceMock.Object);

            var result = await controller.SendMotionAlertLink(new TelegramLinkRequest { ImageUrl = string.Empty, Description = "Test" });

            Assert.IsType<BadRequestObjectResult>(result);
            serviceMock.Verify(s => s.SendImageLinkAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendMotionAlertLink_CallsSendImageLinkAsync_WhenRequestIsValid()
        {
            var serviceMock = new Mock<TelegramService>("bot-token", "123", new HttpClient())
            {
                CallBase = false
            };
            serviceMock.Setup(s => s.SendImageLinkAsync("https://dashboard.example", "Door alert"))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

            var controller = new TelegramBotController(serviceMock.Object);
            var request = new TelegramLinkRequest
            {
                ImageUrl = "https://dashboard.example",
                Description = "Door alert"
            };

            var result = await controller.SendMotionAlertLink(request);

            Assert.IsType<OkObjectResult>(result);
            serviceMock.Verify();
        }
    }
}
