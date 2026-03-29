using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicExample.MultipleSchemes.Controllers
{
    [ApiController]
    [Route("auth")]
    public sealed class AuthController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("public-ok");
        }

        [HttpGet("apikey")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public IActionResult ApiKey()
        {
            return Ok("apikey-ok-" + User?.Identity?.Name);
        }

        [HttpGet("database")]
        [Authorize(AuthenticationSchemes = "Database")]
        public IActionResult Database()
        {
            return Ok("database-ok-" + User?.Identity?.Name);
        }
    }
}
