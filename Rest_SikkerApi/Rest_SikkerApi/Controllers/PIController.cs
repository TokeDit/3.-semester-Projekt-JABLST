using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.repos;
namespace Rest_SikkerApi;
using Rest_SikkerApi.models;

[ApiController]
[Route("api/[controller]")]
public class PIController : ControllerBase
{
    private readonly SikkerRepo m_repo;

    public PIController(SikkerRepo repo)
    {
        m_repo = repo;
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Image image)
    {
        await m_repo.SaveImageAsync(image);
        return Ok("Image saved successfully.");
    }
}
