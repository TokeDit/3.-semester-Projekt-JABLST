using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
using Rest_SikkerApi.models;
namespace Rest_SikkerApi;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly SikkerRepo m_repo;

    public ImageController(SikkerRepo repo)
    {
        m_repo = repo;
    }

    [HttpGet]
    [ProducesResponseType (StatusCodes.Status200OK)]
    [ProducesResponseType (StatusCodes.Status204NoContent)]
    [ProducesResponseType (StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] int? id, [FromQuery] int amount = 20)
    {
        IEnumerable<Image> images;
        if (id.HasValue)
        {
            images = m_repo.GetAfterIDImage(id.Value, amount);
        }
        else
        {
            images = m_repo.GetAmountImage(amount);
        }

        if (images.Count() == 0)
        {
            if (id.HasValue)
            {
                return NotFound($"No images found with ID greater than {id.Value}.");
            }
            return NoContent();
        }
        return Ok(images);
    }
    [HttpGet("user/{ownerUid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByUser(string ownerUid)
    {
        var images = await m_repo.GetImagesByOwnerUidAsync(ownerUid);
        if (!images.Any())
            return NoContent();
        return Ok(images);
    }
}
