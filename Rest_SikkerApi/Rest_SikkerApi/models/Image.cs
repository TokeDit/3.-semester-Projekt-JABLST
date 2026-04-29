namespace Rest_SikkerApi.models
{
    public class Image
    {
        public string Id { get; set; } 

        public string TimeStamp { get; set; } = string.Empty;

        public string ImageType { get; set; } = string.Empty;

        // For JSON serialization/deserialization, use string to hold Base64 data
        public string ImageData { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Helper method to get decoded bytes
        public byte[]? GetImageBytes()
        {
            if (string.IsNullOrEmpty(ImageData))
                return null;

            return Convert.FromBase64String(ImageData);
        }

        // Helper method to set image from bytes
        public void SetImageBytes(byte[] bytes)
        {
            if (bytes == null)
                ImageData = string.Empty;
            else
                ImageData = Convert.ToBase64String(bytes); // converts bytes to string
        }
    }
}
