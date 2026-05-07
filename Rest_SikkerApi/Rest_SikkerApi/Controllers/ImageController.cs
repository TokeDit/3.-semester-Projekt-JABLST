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
}
