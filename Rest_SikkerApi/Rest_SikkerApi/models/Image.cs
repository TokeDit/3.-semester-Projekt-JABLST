using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rest_SikkerApi.models
{
    public class Image
    {
        public int Id { get; set; }

        public string TimeStamp { get; set; } = string.Empty;

        public string ImageType { get; set; } = string.Empty;

        // Store as bytes in database
        public string ImageData { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public float? Confidence { get; set; } = 0f;

        public string DetectedObject { get; set; } = string.Empty;
        
        // Add owner UID (Firebase UID) to tie image to a user
        public string? OwnerUid { get; set; }

        // For API - accept/return Base64 string

        // Helper method to get image bytes (now direct access)
        public string? GetImageBytes()
        {
            return ImageData.Length > 0 ? ImageData : null;
        }

        // Helper method to set image from bytes
        public void SetImageBytes(string bytes)
        {
            ImageData = bytes ?? string.Empty;
        }
    }
}
