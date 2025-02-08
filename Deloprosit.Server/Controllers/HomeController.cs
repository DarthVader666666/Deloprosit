using Microsoft.AspNetCore.Mvc;

namespace Deloprosit.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public async Task<IActionResult> Index()
        {
            return Ok();
        }
    }
}
