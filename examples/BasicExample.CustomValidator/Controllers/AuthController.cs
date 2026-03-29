using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Authentication.Basic;

namespace BasicExample.CustomValidator.Controllers
{
    /// <summary>
    /// Exposes public and Basic-protected endpoints using a custom validator.
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
        /// Returns a protected response when the custom Basic validator accepts credentials.
        /// </summary>
        /// <response code="200">Authenticated request completed successfully.</response>
        /// <response code="401">Missing or invalid Basic credentials.</response>
        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Protected()
        {
            return Ok("protected-ok-" + User?.Identity?.Name);
        }
    }
}
