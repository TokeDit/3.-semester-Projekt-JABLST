using System.ComponentModel.DataAnnotations.Schema;

namespace Rest_SikkerApi.models
{
    public class Image
    {
        public int Id { get; set; }

        public string TimeStamp { get; set; } = string.Empty;

        public string ImageType { get; set; } = string.Empty;

        // Store as bytes in database
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        public string Description { get; set; } = string.Empty;

        public float? Confidence { get; set; } = 0f;

        public string DetectedObject { get; set; } = string.Empty;
        
        // Add owner UID (Firebase UID) to tie image to a user
        public string OwnerUid { get; set; } = string.Empty;

        // For API - accept/return Base64 string

        [NotMapped]
        public string ImageDataBase64
        {
            get => ImageData.Length > 0 ? Convert.ToBase64String(ImageData) : string.Empty;
            set => ImageData = !string.IsNullOrEmpty(value) ? Convert.FromBase64String(value) : Array.Empty<byte>();
        }

        // Helper method to get image bytes (now direct access)
        public byte[]? GetImageBytes()
        {
            return ImageData.Length > 0 ? ImageData : null;
        }

        // Helper method to set image from bytes
        public void SetImageBytes(byte[] bytes)
        {
            ImageData = bytes ?? Array.Empty<byte>();
        }
    }
}
