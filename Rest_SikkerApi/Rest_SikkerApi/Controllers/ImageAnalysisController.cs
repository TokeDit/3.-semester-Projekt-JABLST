// using Microsoft.AspNetCore.Mvc;
// using Rest_SikkerApi.Services;

// namespace Rest_SikkerApi.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public sealed class ImageAnalysisController : ControllerBase
// {
//     // private readonly IImageAnalysisService _imageAnalysisService;

//     public ImageAnalysisController(/*IImageAnalysisService imageAnalysisService*/)
//     {
//         // _imageAnalysisService = imageAnalysisService;
//     }

//     [HttpPost]
//     public async Task<IActionResult> Analyze(IFormFile image, CancellationToken cancellationToken)
//     {
//         try
//         {
//             if (image is null || image.Length == 0)
//             {
//                 return BadRequest("No image uploaded.");
//             }

//             if (!image.ContentType.StartsWith("image/"))
//             {
//                 return BadRequest("Uploaded file must be an image.");
//             }

//             // var result = await _imageAnalysisService.AnalyzeAsync(image, cancellationToken);
//             // return Ok(result);
            
//         }
//         catch (Exception ex)
//         {
//             return StatusCode(500, new
//             {
//                 message = "Image analysis failed.",
//                 error = ex.Message
//             });
//         }
//     }
// }