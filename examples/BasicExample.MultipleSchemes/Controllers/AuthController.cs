using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicExample.MultipleSchemes.Controllers
{
    /// <summary>
    /// Demonstrates endpoints protected by different Basic authentication schemes.
    /// </summary>
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        /// <summary>
        /// Returns a public response without authentication.
        /// </summary>
        /// <response code="200">Public endpoint reached successfully.</response>
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("public-ok");
        }

        /// <summary>
        /// Returns a response for requests authenticated with the ApiKey scheme.
        /// </summary>
        /// <response code="200">Authenticated request completed successfully.</response>
        /// <response code="401">Missing or invalid credentials for ApiKey scheme.</response>
        [HttpGet("apikey")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult ApiKey()
        {
            return Ok("apikey-ok-" + User?.Identity?.Name);
        }

        /// <summary>
        /// Returns a response for requests authenticated with the Database scheme.
        /// </summary>
        /// <response code="200">Authenticated request completed successfully.</response>
        /// <response code="401">Missing or invalid credentials for Database scheme.</response>
        [HttpGet("database")]
        [Authorize(AuthenticationSchemes = "Database")]
        public IActionResult Database()
        {
            return Ok("database-ok-" + User?.Identity?.Name);
        }
    }
}
