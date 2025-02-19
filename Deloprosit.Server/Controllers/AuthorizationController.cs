using AutoMapper;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.Text;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;
        private readonly EmailSender _emailSender;
        private readonly UserManager _userManager;

        public AuthorizationController(
            IRepository<User> userRepository, IMapper mapper, IConfiguration configuration, CryptoService cryptoService, 
            EmailSender emailSender, UserManager userManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _cryptoService = cryptoService;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn([FromQuery]string? nickname = null)
        {
            var userLogInRequestModel = JsonConvert.DeserializeObject<UserLogInRequestModel>(HttpContext.Request.Headers["Authentication"].ToString());
            var password = Encoding.UTF8.GetString(userLogInRequestModel?.Password ?? []);

            var user = await _userManager.GetUserByAsync(nickname: nickname, email: userLogInRequestModel?.Email);

            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
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
                Roles = creds.Value.Roles
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
