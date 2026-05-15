using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Rest_SikkerApi.models;
using Rest_SikkerApi.repos;

namespace Rest_SikkerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly SikkerRepo _repo;

        public AuthController(ILogger<AuthController> logger, SikkerRepo repo)
        {
            _logger = logger;
            _repo = repo;
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
                var uid = decodedToken.Uid;

                var existing = await _repo.GetUserByFirebaseIdAsync(uid);
                if (existing == null)
                    await _repo.SaveUserAsync(new User { OwnerUid = uid });

                return Ok(new
                {
                    firebaseUid = uid,
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
