using Rest_SikkerApi.models;

namespace Rest_SikkerApi.Services;

public interface IImageAnalysisService
{
    Task<ImageAnalysisResult> AnalyzeAsync(IFormFile image, CancellationToken cancellationToken = default);
}
