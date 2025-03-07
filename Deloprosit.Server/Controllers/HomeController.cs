using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("error/{status:int}")]
        public IActionResult Error([FromRoute] int status)
        {
            if (status == 404)
            {
                Redirect(_configuration["ClientUrl"] ?? "/");
            }

            return Ok();
        }
    }
}
