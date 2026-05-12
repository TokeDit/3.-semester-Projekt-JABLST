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

    // if id is provided, get images after that id, otherwise get the latest images.
    // if id is 0 it will be treated as if no id was provided, and the latest images will be returned.
    // id must be a positive integer, otherwise it will return a bad request response.
    [HttpGet]
    [ProducesResponseType (StatusCodes.Status200OK)]
    [ProducesResponseType (StatusCodes.Status204NoContent)]
    [ProducesResponseType (StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] int? id, [FromQuery] int amount = 20)
    {
        if (amount < 0)
        {
            return BadRequest("Amount must be a positive integer.");
        }
        else if (amount > 50)
        {
            amount = 50; // Limit the maximum amount to 50 to prevent excessive data retrieval
        }

        IEnumerable<Image> images;
        if (id.HasValue && id.Value >= 1)
        {
            images = m_repo.GetBeforeIDImage(id.Value, amount);
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

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var image = await m_repo.GetImageByIdAsync(id);
        if (image == null)
            return NotFound($"Image with ID {id} not found.");
        return Ok(image);
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
    // COMMIT: Get monthly event log for a user — issue #151
    [HttpGet("user/{ownerUid}/monthly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetMonthlyLog(
        string ownerUid,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (month == 0) month = DateTime.UtcNow.Month;

        var images = await m_repo.GetImagesByOwnerUidAndMonthAsync(ownerUid, year, month);
        if (!images.Any()) return NoContent();
        return Ok(images);
    }
}
