using System.Text;
using System.Text.Json;
using Rest_SikkerApi.Models;

namespace Rest_SikkerApi.Services;

public sealed class GeminiImageAnalysisService : IImageAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiImageAnalysisService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ImageAnalysisResult> AnalyzeAsync(IFormFile image, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is missing.");
        }

        await using var stream = image.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);

        var base64Image = Convert.ToBase64String(memoryStream.ToArray());

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            text = """
                            Analyze this security camera image.

                            Return ONLY valid JSON in this exact format:
                            {
                              "hasPerson": true,
                              "description": "short Danish description"
                            }

                            Rules:
                            - hasPerson must be true if a person is visible.
                            - description must be Danish.
                            - Do not include markdown.
                            """
                        },
                        new
                        {
                            inline_data = new
                            {
                                mime_type = image.ContentType,
                                data = base64Image
                            }
                        }
                    }
                }
            }
        };

        var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content, cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Gemini API error: {responseText}");
        }

        using var document = JsonDocument.Parse(responseText);

        var aiText = document
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        if (string.IsNullOrWhiteSpace(aiText))
        {
            throw new InvalidOperationException("Gemini returned empty response.");
        }

        aiText = aiText
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var result = JsonSerializer.Deserialize<ImageAnalysisResult>(
            aiText,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result ?? throw new InvalidOperationException("Could not parse Gemini response.");
    }
}