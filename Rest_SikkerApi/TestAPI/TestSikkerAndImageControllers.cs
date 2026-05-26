using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Rest_SikkerApi;
using Rest_SikkerApi.Controllers;
using Rest_SikkerApi.data;
using Rest_SikkerApi.interfaces;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;

namespace TestAPI
{
    // Shared helpers for controllers that need a real SikkerRepo
    internal static class RepoFactory
    {
        internal static DbContextOptions<AppDbContext> InMemoryOptions() =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        internal static (SikkerRepo repo, AppDbContext context) Create()
        {
            var opts = InMemoryOptions();
            var context = new AppDbContext(opts);
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileService = new FileHandlingService(tempDir);
            var dbService = new DatabaseHandlingService(context);
            var repo = new SikkerRepo(context, fileService, dbService);
            return (repo, context);
        }
    }

    // ─── SikkerController ─────────────────────────────────────────────────────

    public class TestSikkerController
    {
        private static SikkerController CreateController(SikkerRepo repo, IFirebaseHandler? firebase = null)
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["DashboardUrl"]).Returns("http://test-dashboard");

            var repoInterfaceMock = new Mock<ISikkerRepo>();
            var botMock = new Mock<TelegramBotService>(
                "token", repoInterfaceMock.Object, new System.Net.Http.HttpClient())
            { CallBase = false };

            return new SikkerController(
                Mock.Of<ILogger<SikkerController>>(),
                repo,
                botMock.Object,
                configMock.Object,
                firebase ?? Mock.Of<IFirebaseHandler>());
        }

        [Fact]
        public void Ping_ReturnsOk()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = controller.Ping();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void TurnOn_ReturnsOk()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = controller.TurnOn();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void TurnOff_ReturnsOk()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = controller.TurnOff();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetStatus_ReturnsOnline_AfterTurnOn()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            controller.TurnOn();

            var result = (OkObjectResult)controller.GetStatus();
            var status = (string)result.Value!.GetType().GetProperty("status")!.GetValue(result.Value)!;

            Assert.Equal("online", status);
            repo.SetSystemState(false); // reset static
        }

        [Fact]
        public void GetStatus_ReturnsOffline_AfterTurnOff()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            controller.TurnOff();

            var result = (OkObjectResult)controller.GetStatus();
            var status = (string)result.Value!.GetType().GetProperty("status")!.GetValue(result.Value)!;

            Assert.Equal("offline", status);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenNoImages()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.Get();
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenImagesExist()
        {
            var (repo, context) = RepoFactory.Create();
            context.Images.Add(new Image { Id = 1, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg" });
            await context.SaveChangesAsync();

            var controller = CreateController(repo);
            var result = await controller.Get();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenNull()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.UploadImage(null!);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenImageDataEmpty()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.UploadImage(new Image());
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

    // ─── ImageController ──────────────────────────────────────────────────────

    public class TestImageControllerClass
    {
        private static ImageController CreateController(SikkerRepo repo, Mock<IFirebaseHandler>? firebaseMock = null)
        {
            firebaseMock ??= new Mock<IFirebaseHandler>();
            return new ImageController(repo, firebaseMock.Object);
        }

        [Fact]
        public async Task GetFixedAmount_Returns401_WhenFirebaseThrowsArgumentNull()
        {
            var (repo, _) = RepoFactory.Create();
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync())
                        .ThrowsAsync(new ArgumentNullException());

            var controller = CreateController(repo, firebaseMock);
            var result = await controller.GetFixedAmount(null, 10);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetFixedAmount_Returns500_WhenFirebaseThrowsInvalidOperation()
        {
            var (repo, _) = RepoFactory.Create();
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync())
                        .ThrowsAsync(new InvalidOperationException());

            var controller = CreateController(repo, firebaseMock);
            var result = await controller.GetFixedAmount(null, 10);

            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetFixedAmount_ReturnsBadRequest_WhenAmountIsNegative()
        {
            var (repo, _) = RepoFactory.Create();
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync()).ReturnsAsync("uid-1");

            var controller = CreateController(repo, firebaseMock);
            var result = await controller.GetFixedAmount(null, -1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetFixedAmount_ReturnsNoContent_WhenNoImages()
        {
            var (repo, _) = RepoFactory.Create();
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync()).ReturnsAsync("uid-empty");

            var controller = CreateController(repo, firebaseMock);
            var result = await controller.GetFixedAmount(null, 10);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Get_ReturnsNoContent_WhenNoImages()
        {
            var (repo, _) = RepoFactory.Create();
            var firebaseMock = new Mock<IFirebaseHandler>();
            firebaseMock.Setup(f => f.GetFirebaseUidAsync()).ReturnsAsync("uid-1");

            var controller = CreateController(repo, firebaseMock);
            var result = await controller.Get();

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequest_WhenIdIsZero()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.Get(0);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByUser_ReturnsNoContent_WhenNoImages()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.GetByUser("no-images-uid");
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetByUser_ReturnsOk_WhenImagesExist()
        {
            var (repo, context) = RepoFactory.Create();
            context.Images.Add(new Image { Id = 50, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "uid-A" });
            await context.SaveChangesAsync();

            var controller = CreateController(repo);
            var result = await controller.GetByUser("uid-A");

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMonthlyLog_ReturnsNoContent_WhenNoImages()
        {
            var (repo, _) = RepoFactory.Create();
            var controller = CreateController(repo);
            var result = await controller.GetMonthlyLog("uid-B", 2026, 3);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetMonthlyLog_ReturnsOk_WhenImagesExist()
        {
            var (repo, context) = RepoFactory.Create();
            context.Images.Add(new Image { Id = 60, TimeStamp = new DateTime(2026, 4, 15), ImageType = "image/jpeg", OwnerUid = "uid-C" });
            await context.SaveChangesAsync();

            var controller = CreateController(repo);
            var result = await controller.GetMonthlyLog("uid-C", 2026, 4);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
