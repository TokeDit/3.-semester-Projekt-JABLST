using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Rest_SikkerApi;
using Rest_SikkerApi.Controllers;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;

namespace TestAPI
{
    // ─── AdminReportsController ───────────────────────────────────────────────

    public class TestAdminReportsController
    {
        [Fact]
        public void Ping_ReturnsOk()
        {
            var controller = new AdminReportsController(
                new Mock<ReportService>(Mock.Of<IServiceScopeFactory>(), Mock.Of<ILogger<ReportService>>()).Object,
                Mock.Of<ILogger<AdminReportsController>>());

            var result = controller.Ping();

            Assert.IsType<OkObjectResult>(result);
        }
    }

    // ─── PIController ─────────────────────────────────────────────────────────

    public class TestPIController
    {
        private static PIController CreateController(
            Mock<ISikkerRepo>? repoMock = null,
            Mock<IFirebaseHandler>? firebaseMock = null)
        {
            repoMock ??= new Mock<ISikkerRepo>();
            firebaseMock ??= new Mock<IFirebaseHandler>();
            var botMock = new Mock<TelegramBotService>(
                "token", repoMock.Object, new System.Net.Http.HttpClient())
            { CallBase = false };
            return new PIController(repoMock.Object, botMock.Object, firebaseMock.Object);
        }

        [Fact]
        public void HeartBeat_ReturnsOk()
        {
            var controller = CreateController();
            var result = controller.HeartBeat(new HeartBeatDto { TimeStamp = DateTime.UtcNow });
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetStatus_ReturnsOk()
        {
            var controller = CreateController();
            var result = controller.GetStatus();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetStatus_IsAlive_AfterHeartBeat()
        {
            var controller = CreateController();
            controller.HeartBeat(new HeartBeatDto { TimeStamp = DateTime.UtcNow });

            var result = (OkObjectResult)controller.GetStatus();
            var value = result.Value!;
            var isAlive = (bool)value.GetType().GetProperty("isAlive")!.GetValue(value)!;

            Assert.True(isAlive);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenImageIsNull()
        {
            var controller = CreateController();
            var result = await controller.Post(null!);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenImageDataIsEmpty()
        {
            var controller = CreateController();
            var result = await controller.Post(new Image());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_Returns400_WhenFirebaseUidIsEmpty()
        {
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync()).ReturnsAsync(string.Empty);

            var controller = CreateController(firebaseMock: firebaseMock);
            var image = new Image();
            image.SetImageBytes(new byte[] { 1, 2, 3 });

            var result = await controller.Post(image);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

    // ─── UserController ───────────────────────────────────────────────────────

    public class TestUserController
    {
        private static UserController Create(Mock<ISikkerRepo> repoMock) =>
            new UserController(repoMock.Object, Mock.Of<ILogger<UserController>>());

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByFirebaseIdAsync("uid-missing")).ReturnsAsync((User?)null);

            var controller = Create(repoMock);
            var result = await controller.GetUser("uid-missing");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUser_ReturnsOk_WhenUserExists()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.GetUserByFirebaseIdAsync("uid-exists"))
                    .ReturnsAsync(new User { OwnerUid = "uid-exists" });

            var controller = Create(repoMock);
            var result = await controller.GetUser("uid-exists");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk_WhenUpdateSucceeds()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.UpdateUserAsync("uid-1", "555", 7, true))
                    .ReturnsAsync(new User { OwnerUid = "uid-1", TelegramChatId = "555" });

            var controller = Create(repoMock);
            var result = await controller.UpdateUser("uid-1",
                new UserController.UpdateUserRequest
                {
                    TelegramChatId = "555",
                    ReportFrequency = 7,
                    ReportEnabled = true
                });

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenRepoReturnsNull()
        {
            var repoMock = new Mock<ISikkerRepo>();
            repoMock.Setup(r => r.UpdateUserAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<bool>()))
                    .ReturnsAsync((User?)null);

            var controller = Create(repoMock);
            var result = await controller.UpdateUser("uid-ghost",
                new UserController.UpdateUserRequest());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
