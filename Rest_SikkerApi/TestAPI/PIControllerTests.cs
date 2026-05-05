using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi;
using Rest_SikkerApi.models;
using Xunit;

namespace TestAPI
{
    public class PIControllerTests
    {
        [Fact]
        public async Task Post_ReturnsBadRequest_WhenImageIsNull()
        {
            // Arrange
            var controller = new PIController(null!);

            // Act
            var result = await controller.Post(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No image uploaded.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenImageDataIsEmpty()
        {
            // Arrange
            var controller = new PIController(null!);

            var image = new Image
            {
                ImageData = Array.Empty<byte>()
            };

            // Act
            var result = await controller.Post(image);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No image uploaded.", badRequestResult.Value);
        }

        [Fact]
        public void HeartBeat_ReturnsOk()
        {
            // Arrange
            var controller = new PIController(null!);

            var heartbeat = new HeartBeatDto();

            // Act
            var result = controller.HeartBeat(heartbeat);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetStatus_ReturnsOkWithStatusObject()
        {
            // Arrange
            var controller = new PIController(null!);

            // Act
            var result = controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetStatus_ReturnsAliveTrue_AfterHeartBeat()
        {
            // Arrange
            var controller = new PIController(null!);
            var heartbeat = new HeartBeatDto();

            // Act
            controller.HeartBeat(heartbeat);
            var result = controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            using var document = System.Text.Json.JsonDocument.Parse(json);

            Assert.True(document.RootElement.GetProperty("isAlive").GetBoolean());
            Assert.NotEqual(
                System.Text.Json.JsonValueKind.Null,
                document.RootElement.GetProperty("lastSeen").ValueKind);
        }
    }
}