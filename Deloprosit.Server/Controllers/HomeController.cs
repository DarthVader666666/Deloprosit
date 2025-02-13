using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("home/index")]
        public async Task<IActionResult> Index([FromQuery] string returnUrl)
        {
            return Ok(returnUrl);
        }
    }
}
