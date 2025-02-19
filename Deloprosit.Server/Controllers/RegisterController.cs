using AutoMapper;

using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
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
        private readonly IMapper _automapper;
        private readonly IConfiguration _configuration;

        public RegisterController(UserManager userManager, EmailSender emailSender, IMapper automaper, IConfiguration configuration)
        {
            _userManager = userManager;
            _automapper = automaper;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var userRegister = JsonConvert.DeserializeObject<RegisterRequestModel>(HttpContext.Request.Headers["Registration"].ToString());

            if (userRegister?.Password == null)
            {
                return BadRequest(new { errorText = "Ошибка регистрации: Не указан пароль" });
            }

            if (userRegister.Email == null)
            {
                return BadRequest(new { errorText = "Ошибка регистрации: Не указан Email"});
            }

            if (await _userManager.DoesUserExistAsync(userRegister.Email))
            {
                return BadRequest(new { errorText = "Такой пользователь уже зарегестрирован" });
            }

            var user = _automapper.Map<User>(userRegister);

            var result = await _userManager.RegisterAsync(user);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Confirm([FromQuery] string? key)
        {
            var confirmedUser = await _userManager.ConfirmUserAsync(key);

            if (confirmedUser == null)
            {
                return Problem("Ошибка подтверждения", statusCode: 500);
            }

            return Redirect($"{_configuration["ClientUrl"]}/registration?confirmed=true&nickname={confirmedUser.Nickname}");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UserExists([FromQuery] string? nickname, [FromQuery] string? email) 
        {
            var userExists = false;

            if (nickname == null)
            {
                userExists = await _userManager.DoesUserExistAsync(email, doEncrypt: true);
            }
            else
            {
                userExists = await _userManager.DoesUserExistAsync(nickname);
            }

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
