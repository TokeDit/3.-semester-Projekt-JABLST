using Microsoft.AspNetCore.Mvc;
// COMMIT 1:
using Rest_SikkerApi.interfaces;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rest_SikkerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        // GET: api/<TelegramController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TelegramController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TelegramController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TelegramController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TelegramController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
