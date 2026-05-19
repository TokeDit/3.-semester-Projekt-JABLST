using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ISikkerRepo _repo;
        private readonly ILogger<UserController> _logger;

        public UserController(ISikkerRepo repo, ILogger<UserController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("{ownerUid}")]
        public async Task<IActionResult> GetUser(string ownerUid)
        {
            //  Skip Firebase auth locally when DefaultInstance is null
            if (FirebaseAuth.DefaultInstance != null)
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized("Missing Authorization header.");

                var idToken = authHeader["Bearer ".Length..];
                try
                {
                    FirebaseToken decodedToken = await FirebaseAuth
                        .DefaultInstance.VerifyIdTokenAsync(idToken);
                    if (decodedToken.Uid != ownerUid)
                        return Forbid();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid Firebase token");
                    return Unauthorized("Invalid Firebase token.");
                }
            }

            var user = await _repo.GetUserByFirebaseIdAsync(ownerUid);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Updated PUT endpoint to allow local testing without Firebase auth
        [HttpPut("{ownerUid}")]
        public async Task<IActionResult> UpdateUser(string ownerUid, [FromBody] UpdateUserRequest request)
        {
            // Skip Firebase auth locally when DefaultInstance is null
            if (FirebaseAuth.DefaultInstance != null)
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                    return Unauthorized("Missing Authorization header.");

                var idToken = authHeader["Bearer ".Length..];
                try
                {
                    FirebaseToken decodedToken = await FirebaseAuth
                        .DefaultInstance.VerifyIdTokenAsync(idToken);
                    if (decodedToken.Uid != ownerUid)
                        return Forbid();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid Firebase token");
                    return Unauthorized("Invalid Firebase token.");
                }
            }

            var updated = await _repo.UpdateUserAsync(
                ownerUid,
                request.TelegramChatId,
                request.ReportFrequency,
                request.ReportEnabled);

            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // Request model for updating user preferences
        public class UpdateUserRequest
        {
            public string? TelegramChatId { get; set; }
            public int ReportFrequency { get; set; } = 7; // 1=daily, 7=weekly, 30=monthly
            public bool ReportEnabled { get; set; } = true;
        }
    }
}