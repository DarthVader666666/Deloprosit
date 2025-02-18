using AutoMapper;

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
        private readonly EmailSender _emailSender;
        private readonly IMapper _automapper;
        private readonly IConfiguration _configuration;

        public RegisterController(UserManager userManager, EmailSender emailSender, IMapper automaper, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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

            //if (await _userManager.DoesUserExistAsync(userRegister.Email))
            //{
            //    return BadRequest(new { errorText = "Такой пользователь уже зарегестрирован" });
            //}

            //var user = _automapper.Map<RegisterRequestModel>(userRegister);

            //var url = $"<button>" +
            //    $"<a href='{_configuration["ClientUrl"]}/registration/confirm?" +
            //    $"key={userRegister.Email}&encryptedPassword={userRegister.Password}" +
            //    $"&firstName={userRegister.FirstName}&lastName={userRegister.LastName}' " +
            //    $"style=\"text-decoration: none; color: black\">" +
            //    $"Confirm Registration" +
            //    $"</a>" +
            //    $"</button>";

            //var result = await _emailSender.SendEmailAsync(userRegister.Email, "Please, confirm Your registration", url);

            if (true)//result)
            {
                return Ok(new { message = "Email sent" });
            }
            else
            {
                return BadRequest("Error while sending email");
            }
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
