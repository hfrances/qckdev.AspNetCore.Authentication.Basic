using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace qckdev.AspNetCore.Authentication.Basic.Test
{
    [ApiController]
    [Route("basic")]
    public sealed class BasicAuthenticationTestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("public-ok");
        }

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Protected()
        {
            return Ok($"protected-ok-{User?.Identity?.Name}");
        }

        [HttpGet("protected-secondary")]
        [Authorize(AuthenticationSchemes = "BasicSecondary")]
        public IActionResult ProtectedSecondary()
        {
            return Ok($"protected-secondary-ok-{User?.Identity?.Name}");
        }
    }
}
