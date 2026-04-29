using Microsoft.EntityFrameworkCore;
using Rest_SikkerApi.data;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;

namespace TestAPI
{
    public class TestImgRepo
    {
        private DbContextOptions<AppDbContext> CreateInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task SaveImageAsync_ShouldSaveImageToDatabase()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var imageEntity = new Image
            {
                Id = "test-image-1",
                TimeStamp = "2026-04-29T10:00:00",
                ImageType = "image/jpeg",
                ImageData = new byte[] { 1, 2, 3, 4, 5 },
                Description = "Test image"
            };

            // Act
            var result = await repo.SaveImageAsync(imageEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-image-1", result.Id);
            Assert.Equal("2026-04-29T10:00:00", result.TimeStamp);
            Assert.Equal("image/jpeg", result.ImageType);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, result.ImageData);
            Assert.Equal("Test image", result.Description);

            // Verify it's actually in the database
            var savedImage = await context.Images.FindAsync("test-image-1");
            Assert.NotNull(savedImage);
            Assert.Equal("test-image-1", savedImage.Id);
        }

        [Fact]
        public async Task SaveImageAsync_ShouldReturnImageWithAllProperties()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var imageEntity = new Image
            {
                Id = "test-image-2",
                TimeStamp = "2026-04-29T11:30:00",
                ImageType = "image/png",
                ImageData = new byte[] { 10, 20, 30 },
                Description = "Another test image"
            };

            // Act
            var result = await repo.SaveImageAsync(imageEntity);

            // Assert
            Assert.Same(imageEntity, result);
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnEmptyList_WhenNoImagesExist()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            // Act
            var result = await repo.GetAllImagesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnSingleImage_WhenOneImageExists()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var imageEntity = new Image
            {
                Id = "single-image",
                TimeStamp = "2026-04-29T12:00:00",
                ImageType = "image/gif",
                ImageData = new byte[] { 5, 10, 15 },
                Description = "Single image"
            };
            await repo.SaveImageAsync(imageEntity);

            // Act
            var result = await repo.GetAllImagesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("single-image", result[0].Id);
            Assert.Equal("2026-04-29T12:00:00", result[0].TimeStamp);
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnAllImages_WhenMultipleImagesExist()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var image1 = new Image
            {
                Id = "image-1",
                TimeStamp = "2026-04-29T13:00:00",
                ImageType = "image/jpeg",
                ImageData = new byte[] { 1, 2, 3 },
                Description = "First image"
            };

            var image2 = new Image
            {
                Id = "image-2",
                TimeStamp = "2026-04-29T14:00:00",
                ImageType = "image/png",
                ImageData = new byte[] { 4, 5, 6 },
                Description = "Second image"
            };

            var image3 = new Image
            {
                Id = "image-3",
                TimeStamp = "2026-04-29T15:00:00",
                ImageType = "image/webp",
                ImageData = new byte[] { 7, 8, 9 },
                Description = "Third image"
            };

            await repo.SaveImageAsync(image1);
            await repo.SaveImageAsync(image2);
            await repo.SaveImageAsync(image3);

            // Act
            var result = await repo.GetAllImagesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, i => i.Id == "image-1");
            Assert.Contains(result, i => i.Id == "image-2");
            Assert.Contains(result, i => i.Id == "image-3");
        }

        [Fact]
        public async Task GetAllImagesAsync_ShouldReturnImagesWithCorrectData()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var expectedImageData = new byte[] { 100, 101, 102, 103 };
            var imageEntity = new Image
            {
                Id = "data-test",
                TimeStamp = "2026-04-29T16:00:00",
                ImageType = "image/bmp",
                ImageData = expectedImageData,
                Description = "Data validation test"
            };
            await repo.SaveImageAsync(imageEntity);

            // Act
            var result = await repo.GetAllImagesAsync();

            // Assert
            var retrievedImage = result.First();
            Assert.Equal(expectedImageData, retrievedImage.ImageData);
            Assert.Equal("Data validation test", retrievedImage.Description);
        }

        [Fact]
        public async Task SaveImageAsync_ShouldHandleEmptyImageData()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var imageEntity = new Image
            {
                Id = "empty-data-image",
                TimeStamp = "2026-04-29T17:00:00",
                ImageType = "image/jpeg",
                ImageData = Array.Empty<byte>(),
                Description = "Empty data image"
            };

            // Act
            var result = await repo.SaveImageAsync(imageEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.ImageData);
        }

        [Fact]
        public async Task SaveImageAsync_ShouldHandleLargeImageData()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            using var context = new AppDbContext(options);
            var repo = new SikkerRepo(context);

            var largeImageData = new byte[1024 * 100]; // 100KB
            for (int i = 0; i < largeImageData.Length; i++)
            {
                largeImageData[i] = (byte)(i % 256);
            }

            var imageEntity = new Image
            {
                Id = "large-image",
                TimeStamp = "2026-04-29T18:00:00",
                ImageType = "image/jpeg",
                ImageData = largeImageData,
                Description = "Large image test"
            };

            // Act
            var result = await repo.SaveImageAsync(imageEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1024 * 100, result.ImageData.Length);
        }
    }
}
