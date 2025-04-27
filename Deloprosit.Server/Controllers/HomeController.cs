using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
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

        [Route("/error/{status}")]
        public IActionResult Error(int status)
        {
            if (status == 404)
            {
                return Redirect(_configuration["ClientUrl"] ?? "/#/");
            }

            return Ok();
        }

        //[Route("/")]
        //public IActionResult RedirectHome()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/");
        //}

        //[Route("chapters/{chapterId:int}/{themeId:int?}")]
        //public IActionResult RedirectToTheme(int? chapterId, int? themeId)
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/chapters/{chapterId}/{themeId}");
        //}

        //[Route("chapters/create")]
        //public IActionResult RedirectToCreateChapter()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/chapters/create");
        //}

        //[Route("feedback")]
        //public IActionResult RedirectToFeedback()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/feedback");
        //}

        //[Route("register")]
        //public IActionResult RedirectToRegister()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/register");
        //}

        //[Route("messages")]
        //public IActionResult RedirectToMessages()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/messages");
        //}

        //[Route("search-result")]
        //public IActionResult RedirectToSearchResult()
        //{
        //    return Redirect($"{_configuration["ClientUrl"]}/search-result");
        //}
    }
}
