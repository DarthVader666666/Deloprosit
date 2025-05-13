using Delopro.Bll.Interfaces;
using Delopro.Bll.Services;
using Delopro.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Security.Claims;
using System.Text;

namespace Delopro.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IEmailSender _emailSender;

        public AuthenticationController(UserManager userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
                return Ok(new { okText = "Пользователь не аутентифицирован" });
            }

            var claims = user.Claims;

            return Ok(new UserLogInResponseModel()
            {
                Nickname = claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value,
                Roles = claims.Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Select(x => x.Value).ToArray(),
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
                return StatusCode(500, new { errorText = $"Logout failed. {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RecoverPassword()
        {
            var email = HttpContext.Request.Headers["Email"].ToString();
            var userExists = await _userManager.DoesUserExistAsync(email, doEncrypt: true);

            if (!userExists)
            {
                return BadRequest(new { errorText = $"Пользователь с email \"{email}\" не найден" });
            }

            var password = _userManager.GeneratePassword();

            if (!_emailSender.SendEmail(email, "Восстановление пароля", $"Ваш новый пароль:\n\r{password}"))
            {
                return StatusCode(500, new { errorText = "Ошибка отправки сообщения" });
            }

            try
            {
                var user = await _userManager.GetUserByAsync(email: email);
                await _userManager.ChangePasswordAsync(user, password);
            }
            catch
            {
                return StatusCode(500, new { errorText = "Ошибка при изменении пароля" });
            }            

            return Ok("Сообщение успешно отправлено");
        }
    }
}
