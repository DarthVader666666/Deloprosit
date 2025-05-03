using Delopro.Bll;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("/error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                return Redirect(_configuration["ClientUrl"] ?? "/");
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetImageNames()
        {
            var imageNames = Directory.GetFiles(ConfigurationHelper.ChapterImagesPath!).Select(x => x.Split('\\').Last());
            return Ok(imageNames);
        }
    }
}
