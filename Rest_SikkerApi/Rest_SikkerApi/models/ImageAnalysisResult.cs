namespace Rest_SikkerApi.models;

public sealed class ImageAnalysisResult
{
    public bool HasPerson { get; set; }
    public string Description { get; set; } = string.Empty;
}
