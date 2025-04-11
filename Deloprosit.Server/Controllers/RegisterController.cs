using System.Text;

using AutoMapper;

using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IMapper _automapper;
        private readonly IConfiguration _configuration;

        public RegisterController(UserManager userManager, IMapper automaper, IConfiguration configuration)
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

            try
            {
                var user = _automapper.Map<User>(userRegister);

                var result = await _userManager.RegisterAsync(user, HttpContext.Request.GetDisplayUrl());

                if (result)
                {
                    return Ok(new { okText = "Письмо отправлено" });
                }
                else
                {
                    return BadRequest(new { errorText = "Ошибка регистрации"});
                }
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new { errorText = ex.Message });
            }

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Confirm([FromQuery] int[]? key1, [FromQuery] int[]? key2)
        {
            var confirmedUser = await _userManager.ConfirmUserAsync(
                [
                    Encoding.UTF8.GetString((key1 ?? []).Select(x => (byte)x).ToArray()), 
                    Encoding.UTF8.GetString((key2 ?? []).Select(x => (byte)x).ToArray())
                ]
            );

            if (confirmedUser == null)
            {
                return StatusCode(500, new { errorText = "Ошибка подтверждения" });
            }

            await _userManager.LogIn(confirmedUser, HttpContext);

            return Redirect($"{_configuration["ClientUrl"]}");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UserExists([FromQuery] string? nickname, [FromQuery] string? email) 
        {
            bool userExists;

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
