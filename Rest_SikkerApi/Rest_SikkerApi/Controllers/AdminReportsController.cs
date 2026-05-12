using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rest_SikkerApi.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("api/admin/reports")]
    public class AdminReportsController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly ILogger<AdminReportsController> _logger;

        public AdminReportsController(ReportService reportService, ILogger<AdminReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        // Quick ping to verify controller is reachable
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { status = "admin-reports alive" });
    }
}
