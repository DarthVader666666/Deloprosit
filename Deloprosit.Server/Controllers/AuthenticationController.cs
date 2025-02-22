using Deloprosit.Bll.Services;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Security.Claims;
using System.Text;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager _userManager;

        public AuthenticationController(UserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn([FromQuery]string? nickname = null, [FromQuery] bool? remember = false)
        {
            var userLogInRequestModel = JsonConvert.DeserializeObject<UserLogInRequestModel>(HttpContext.Request.Headers["Authentication"].ToString());
            var password = Encoding.UTF8.GetString(userLogInRequestModel?.Password ?? []);

            var user = await _userManager.GetUserByAsync(nickname: nickname, email: userLogInRequestModel?.Email);

            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }

            if (!user.IsConfirmed)
            {
                return NotFound(new { errorText = "Пользователь не подтвержден" });
            }

            if (!_userManager.IsMatchPassword(user, password))
            {
                return BadRequest(new { errorText = "Неверный пароль" });
            }

            var creds = await _userManager.LogIn(user, HttpContext);

            if (creds == null || !creds.Value.Roles.Any())
            {
                return BadRequest(new { errorText = "Couldn't get user identity." });
            }

            return Ok(new UserLogInResponseModel()
            {
                Nickname = creds.Value.Nickname,
                Roles = creds.Value.Roles,
                Remember = remember
            });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult CookieCredentials()
        {
            var user = HttpContext.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Ok("Пользователь не аутентифицирован");
            }

            var claims = user.Claims;

            return Ok(new UserLogInResponseModel()
            {
                Nickname = claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value,
                Roles = claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value.Split(','),
                IsAuthenticated = user.Identity.IsAuthenticated
            });
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _userManager.LogOut(HttpContext);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem($"Logout failed. {ex.Message}", statusCode: 500);
            }
        }
    }
}
