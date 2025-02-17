using Deloprosit.Bll.Services;
using Microsoft.AspNetCore.Mvc;

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
