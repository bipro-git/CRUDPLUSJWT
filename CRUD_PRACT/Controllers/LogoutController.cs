using CRUD_PRACT.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_PRACT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string token)
        {
           BlockToken.addBlacklist(token);
            return Ok("Token has been blacklisted");
        }
    }
}
