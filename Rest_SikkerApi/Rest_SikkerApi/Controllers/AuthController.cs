using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var authHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Missing Authorization header.");
            }

            var idToken = authHeader["Bearer ".Length..];

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                return Ok(new
                {
                    firebaseUid = decodedToken.Uid,
                    email = decodedToken.Claims.TryGetValue("email", out var email)
                        ? email?.ToString()
                        : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid Firebase token");
                return Unauthorized("Invalid Firebase token.");
            }
        }
    }
}