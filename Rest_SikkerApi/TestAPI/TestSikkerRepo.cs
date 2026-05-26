using Microsoft.EntityFrameworkCore;
using Moq;
using Rest_SikkerApi;
using Rest_SikkerApi.data;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;

namespace TestAPI
{
    public class TestSikkerRepo
    {
        private static DbContextOptions<AppDbContext> CreateInMemoryOptions() =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private static (SikkerRepo repo, AppDbContext context) CreateRepo()
        {
            var options = CreateInMemoryOptions();
            var context = new AppDbContext(options);
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var fileHandlingService = new FileHandlingService(tempDir);
            var dbHandlingService = new DatabaseHandlingService(context);
            var repo = new SikkerRepo(context, fileHandlingService, dbHandlingService);
            return (repo, context);
        }

        // --- SikkerRepo: GetAllImagesAsync ---

        [Fact]
        public async Task GetAllImagesAsync_ReturnsEmpty_WhenNoImages()
        {
            var (repo, _) = CreateRepo();
            var result = await repo.GetAllImagesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllImagesAsync_ReturnsAll_WhenImagesExist()
        {
            var (repo, context) = CreateRepo();
            context.Images.AddRange(
                new Image { Id = 1, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "uid1" },
                new Image { Id = 2, TimeStamp = DateTime.UtcNow, ImageType = "image/png", OwnerUid = "uid2" }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetAllImagesAsync();
            Assert.Equal(2, result.Count);
        }

        // --- SikkerRepo: SaveUserAsync ---

        [Fact]
        public async Task SaveUserAsync_PersistsUser()
        {
            var (repo, context) = CreateRepo();
            var user = new User { OwnerUid = "firebase-uid-1", TelegramChatId = null };

            await repo.SaveUserAsync(user);

            var saved = await context.Users.FindAsync("firebase-uid-1");
            Assert.NotNull(saved);
            Assert.Equal("firebase-uid-1", saved.OwnerUid);
        }

        // --- SikkerRepo: GetUserByFirebaseIdAsync ---

        [Fact]
        public async Task GetUserByFirebaseIdAsync_ReturnsUser_WhenExists()
        {
            var (repo, context) = CreateRepo();
            context.Users.Add(new User { OwnerUid = "uid-abc", TelegramChatId = "12345" });
            await context.SaveChangesAsync();

            var result = await repo.GetUserByFirebaseIdAsync("uid-abc");
            Assert.NotNull(result);
            Assert.Equal("12345", result!.TelegramChatId);
        }

        [Fact]
        public async Task GetUserByFirebaseIdAsync_ReturnsNull_WhenNotFound()
        {
            var (repo, _) = CreateRepo();
            var result = await repo.GetUserByFirebaseIdAsync("nonexistent-uid");
            Assert.Null(result);
        }

        // --- SikkerRepo: UpdateUserChatIdAsync ---

        [Fact]
        public async Task UpdateUserChatIdAsync_ReturnsFalse_WhenUserNotFound()
        {
            var (repo, _) = CreateRepo();
            var result = await repo.UpdateUserChatIdAsync("no-such-uid", "999");
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserChatIdAsync_UpdatesChatId_WhenUserExists()
        {
            var (repo, context) = CreateRepo();
            context.Users.Add(new User { OwnerUid = "uid-xyz" });
            await context.SaveChangesAsync();

            var result = await repo.UpdateUserChatIdAsync("uid-xyz", "777777");

            Assert.True(result);
            var user = await context.Users.FindAsync("uid-xyz");
            Assert.Equal("777777", user!.TelegramChatId);
        }

        // --- SikkerRepo: GetUserByChatIdAsync ---

        [Fact]
        public async Task GetUserByChatIdAsync_ReturnsUser_WhenChatIdMatches()
        {
            var (repo, context) = CreateRepo();
            context.Users.Add(new User { OwnerUid = "uid-chat", TelegramChatId = "555" });
            await context.SaveChangesAsync();

            var result = await repo.GetUserByChatIdAsync("555");
            Assert.NotNull(result);
            Assert.Equal("uid-chat", result!.OwnerUid);
        }

        [Fact]
        public async Task GetUserByChatIdAsync_ReturnsNull_WhenNoMatch()
        {
            var (repo, _) = CreateRepo();
            var result = await repo.GetUserByChatIdAsync("no-such-chat-id");
            Assert.Null(result);
        }

        // --- SikkerRepo: GetSystemState / SetSystemState ---

        [Fact]
        public void SetSystemState_True_ThenGetSystemState_ReturnsTrue()
        {
            var (repo, _) = CreateRepo();
            repo.SetSystemState(true);
            Assert.True(repo.GetSystemState());
            repo.SetSystemState(false); // reset static field
        }

        [Fact]
        public void SetSystemState_False_ThenGetSystemState_ReturnsFalse()
        {
            var (repo, _) = CreateRepo();
            repo.SetSystemState(false);
            Assert.False(repo.GetSystemState());
        }

        [Fact]
        public void SetSystemState_ReturnsUpdatedState()
        {
            var (repo, _) = CreateRepo();
            Assert.True(repo.SetSystemState(true));
            Assert.False(repo.SetSystemState(false));
        }

        // --- SikkerRepo: GetImagesByOwnerUidAsync ---

        [Fact]
        public async Task GetImagesByOwnerUidAsync_ReturnsOnlyMatchingOwner()
        {
            var (repo, context) = CreateRepo();
            context.Images.AddRange(
                new Image { Id = 10, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "owner-A" },
                new Image { Id = 11, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "owner-B" },
                new Image { Id = 12, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "owner-A" }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetImagesByOwnerUidAsync("owner-A");
            Assert.Equal(2, result.Count);
            Assert.All(result, img => Assert.Equal("owner-A", img.OwnerUid));
        }

        // --- SikkerRepo: GetUsersWithReportsEnabledAsync ---

        [Fact]
        public async Task GetUsersWithReportsEnabledAsync_ReturnsOnlyEnabledWithChatId()
        {
            var (repo, context) = CreateRepo();
            context.Users.AddRange(
                new User { OwnerUid = "u1", TelegramChatId = "100", ReportEnabled = true },
                new User { OwnerUid = "u2", TelegramChatId = null, ReportEnabled = true },
                new User { OwnerUid = "u3", TelegramChatId = "200", ReportEnabled = false }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetUsersWithReportsEnabledAsync();
            Assert.Single(result);
            Assert.Equal("u1", result[0].OwnerUid);
        }

        // --- SikkerRepo: GetImagesByOwnerUidAndMonthAsync ---

        [Fact]
        public async Task GetImagesByOwnerUidAndMonthAsync_FiltersCorrectly()
        {
            var (repo, context) = CreateRepo();
            context.Images.AddRange(
                new Image { Id = 20, TimeStamp = new DateTime(2026, 3, 15), ImageType = "image/jpeg", OwnerUid = "owner-X" },
                new Image { Id = 21, TimeStamp = new DateTime(2026, 4, 10), ImageType = "image/jpeg", OwnerUid = "owner-X" },
                new Image { Id = 22, TimeStamp = new DateTime(2026, 3, 20), ImageType = "image/jpeg", OwnerUid = "owner-X" }
            );
            await context.SaveChangesAsync();

            var result = await repo.GetImagesByOwnerUidAndMonthAsync("owner-X", 2026, 3);
            Assert.Equal(2, result.Count);
            Assert.All(result, img => Assert.Equal(3, img.TimeStamp.Month));
        }
    }

    public class TestDatabaseHandlingService
    {
        private static DbContextOptions<AppDbContext> CreateInMemoryOptions() =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        [Fact]
        public async Task SaveImageAsync_SetsImagePathAndPersists()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);
            var image = new Image { Id = 1, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg" };

            await service.SaveImageAsync(image);

            var saved = await context.Images.FindAsync(1);
            Assert.NotNull(saved);
            Assert.Equal("images", saved!.ImagePath);
        }

        [Fact]
        public async Task CheckIdUidMatch_ReturnsFalse_WhenImageNotFound()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);

            var result = await service.CheckIdUidMatch(999, "some-uid");
            Assert.False(result);
        }

        [Fact]
        public async Task CheckIdUidMatch_ReturnsFalse_WhenUidMismatch()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);
            context.Images.Add(new Image { Id = 5, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "correct-uid" });
            await context.SaveChangesAsync();

            var result = await service.CheckIdUidMatch(5, "wrong-uid");
            Assert.False(result);
        }

        [Fact]
        public async Task CheckIdUidMatch_ReturnsTrue_WhenMatch()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);
            context.Images.Add(new Image { Id = 7, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "uid-7" });
            await context.SaveChangesAsync();

            var result = await service.CheckIdUidMatch(7, "uid-7");
            Assert.True(result);
        }

        [Fact]
        public async Task GetAmountImage_ReturnsCorrectCount()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);
            for (int i = 1; i <= 5; i++)
                context.Images.Add(new Image { Id = i, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "uid-amt" });
            await context.SaveChangesAsync();

            var result = service.GetAmountImage("uid-amt", 3).ToList();
            Assert.Equal(3, result.Count);
            Assert.All(result, img => Assert.Equal("uid-amt", img.OwnerUid));
        }

        [Fact]
        public async Task GetBeforeIDImage_ReturnsImagesWithIdLessThan()
        {
            var context = new AppDbContext(CreateInMemoryOptions());
            var service = new DatabaseHandlingService(context);
            for (int i = 1; i <= 5; i++)
                context.Images.Add(new Image { Id = i, TimeStamp = DateTime.UtcNow, ImageType = "image/jpeg", OwnerUid = "uid-before" });
            await context.SaveChangesAsync();

            var result = service.GetBeforeIDImage("uid-before", 4, 10).ToList();
            Assert.Equal(3, result.Count);
            Assert.All(result, img => Assert.True(img.Id < 4));
        }
    }
}
