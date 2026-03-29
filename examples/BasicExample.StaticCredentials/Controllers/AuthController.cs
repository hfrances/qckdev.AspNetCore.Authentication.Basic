using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using qckdev.AspNetCore.Authentication.Basic;

namespace BasicExample.StaticCredentials.Controllers
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

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = BasicAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Protected()
        {
            return Ok("protected-ok-" + User?.Identity?.Name);
        }
    }
}
