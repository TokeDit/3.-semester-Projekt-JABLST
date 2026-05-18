
// using Rest_SikkerApi.models;

// namespace TestAPI
// {
//     public class TestImgClass
//     {
//         [Fact]
//         public void ImageDataBase64_Get_ReturnsEmptyString_WhenImageDataIsEmpty()
//         {
//             // Arrange
//             var image = new Image();

//             // Act
//             var result = image.ImageData;

//             // Assert
//             Assert.Equal(string.Empty, result);
//         }

//         [Fact]
//         public void ImageDataBase64_Get_ReturnsBase64String_WhenImageDataHasBytes()
//         {
//             // Arrange
//             var image = new Image();
//             var testBytes = new byte[] { 1, 2, 3, 4, 5 };
//             image.SetImageBytes(testBytes);

//             // Act
//             var result = image.ImageData;

//             // Assert
//             var expectedBase64 = Convert.ToBase64String(testBytes);
//             Assert.Equal(expectedBase64, result);
//         }

//         [Fact]
//         public void ImageDataBase64_Set_SetsImageDataFromBase64String()
//         {
//             // Arrange
//             var image = new Image();
//             var testBytes = new byte[] { 10, 20, 30, 40, 50 };
//             var base64String = Convert.ToBase64String(testBytes);

//             // Act
//             image.ImageData = base64String;

//             // Assert
//             Assert.Equal(testBytes, image.GetImageBytes());
//         }

//         [Fact]
//         public void ImageDataBase64_Set_SetsEmptyArray_WhenValueIsNull()
//         {
//             // Arrange
//             var image = new Image();

//             // Act
//             image.SetImageBytes(new byte[]{});

//             // Assert
//             Assert.Empty(image.ImageData);
//         }

//         [Fact]
//         public void ImageDataBase64_Set_SetsEmptyArray_WhenValueIsEmptyString()
//         {
//             // Arrange
//             var image = new Image();

//             // Act
//             image.ImageData = string.Empty;

//             // Assert
//             Assert.Empty(image.ImageData);
//         }

//         [Fact]
//         public void GetImageBytes_ReturnsNull_WhenImageDataIsEmpty()
//         {
//             // Arrange
//             var image = new Image();

//             // Act
//             var result = image.GetImageBytes();

//             // Assert
//             Assert.Empty(result);
//         }

//         [Fact]
//         public void GetImageBytes_ReturnsBytes_WhenImageDataHasContent()
//         {
//             // Arrange
//             var image = new Image();
//             var testBytes = new byte[] { 100, 200, 255 };
//             image.SetImageBytes(testBytes);

//             // Act
//             var result = image.GetImageBytes();

//             // Assert
//             Assert.NotNull(result);
//             Assert.Equal(testBytes, result);
//         }

//         [Fact]
//         public void SetImageBytes_SetsImageData_WhenBytesProvided()
//         {
//             // Arrange
//             var image = new Image();
//             var testBytes = new byte[] { 50, 100, 150, 200, 250 };

//             // Act
//             image.SetImageBytes(testBytes);

//             // Assert
//             Assert.Equal(testBytes, image.GetImageBytes());
//         }

//         [Fact]
//         public void SetImageBytes_SetsEmptyArray_WhenNullProvided()
//         {
//             // Arrange
//             var image = new Image();
//             // Assert
//             Assert.Empty(image.ImageData);
//         }

//         [Fact]
//         public void Image_InitializesWithDefaultValues()
//         {
//             // Act
//             var image = new Image();

//             // Assert
//             Assert.Equal(new DateTime(), image.TimeStamp);
//             Assert.Equal(string.Empty, image.ImageType);
//             Assert.Empty(image.ImageData);
//             Assert.Equal(string.Empty, image.Description);
//         }

//         [Fact]
//         public void Image_Properties_CanBeSetAndRetrieved()
//         {
//             // Arrange
//             var image = new Image();
//             var testId = 123;
//             var testTimestamp = "2026-04-29T10:30:00";
//             var testImageType = "image/png";
//             var testDescription = "Test image description";

//             // Act
//             image.Id = testId;
//             image.TimeStamp = DateTime.Parse(testTimestamp);
//             image.ImageType = testImageType;
//             image.Description = testDescription;

//             // Assert
//             Assert.Equal(testId, image.Id);
//             Assert.Equal(DateTime.Parse(testTimestamp), image.TimeStamp);
//             Assert.Equal(testImageType, image.ImageType);
//             Assert.Equal(testDescription, image.Description);
//         }

//         [Fact]
//         public void ImageDataBase64_RoundTrip_PreservesData()
//         {
//             // Arrange
//             var image = new Image();
//             var originalBytes = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

//             // Act
//             image.SetImageBytes(originalBytes);
//             var base64 = image.ImageData;
//             var newImage = new Image { ImageData = base64 };

//             // Assert
//             Assert.Equal(originalBytes, newImage.GetImageBytes());
//         }
//     }
// }
