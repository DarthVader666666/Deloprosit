using Deloprosit.Bll.Services;
using Deloprosit.Server.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Deloprosit.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager _userManager;

        public RegisterController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var userRegister = JsonConvert.DeserializeObject<RegisterRequestModel>(HttpContext.Request.Headers["Register"].ToString());
            return Ok();
        }

        [HttpGet]
        [Route("[action]/{nicknameOrEmail}")]
        public async Task<IActionResult> UserExists([FromRoute] string? nicknameOrEmail) 
        {
            var userExists = await _userManager.DoesUserExistAsync(nicknameOrEmail);

            if (userExists)
            {
                return Ok(new { userExists = true }); 
            }
            else
            {
                return Ok(new { userExists = false });
            }
        }
    }
}
