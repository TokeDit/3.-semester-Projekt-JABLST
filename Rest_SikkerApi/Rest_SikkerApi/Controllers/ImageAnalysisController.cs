using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.Services;
using Rest_SikkerApi.models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rest_SikkerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImageAnalysisController : ControllerBase
{
    private readonly IImageAnalysisService _imageAnalysisService;
    private readonly SikkerRepo _sikkerRepo;

    public ImageAnalysisController(IImageAnalysisService imageAnalysisService, SikkerRepo sikkerRepo)
    {
        _imageAnalysisService = imageAnalysisService;
        _sikkerRepo = sikkerRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(IFormFile image, CancellationToken cancellationToken)
    {
        try
        {
            if (image is null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            // tilgiver FirebaseUid i både HttpContext og User.Claims for at sikre kompatibilitet med forskellige autentificeringsmetoder
            var firebaseUid = HttpContext.Items["FirebaseUid"] as string ?? User.FindFirst("firebase_uid")?.Value ?? string.Empty;

            if (!image.ContentType.StartsWith("image/"))
            {
                return BadRequest("Uploaded file must be an image.");
            }

            // Read uploaded file into bytes
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms, cancellationToken);
                imageBytes = ms.ToArray();
            }

            // Create Image entity, set OwnerUid and save
            var imageEntity = new Image
            {
                Id = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.UtcNow.ToString("o"),
                ImageType = image.ContentType,
                ImageData = imageBytes,
                Description = Path.GetFileName(image.FileName) ?? string.Empty,
                OwnerUid = firebaseUid
            };

            await _sikkerRepo.SaveImageAsync(imageEntity);

            // Run analysis on the original uploaded file
            var result = await _imageAnalysisService.AnalyzeAsync(image, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Image analysis failed.",
                error = ex.Message
            });
        }
    }
}