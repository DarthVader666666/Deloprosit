using AutoMapper;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Deloprosit.Bll.Services;
using Azure.Communication.Email;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;
        private readonly EmailSender _emailSender;
        const char rolesSeperator = ',';

        public AuthorizationController(
            IRepository<User> userRepository, IRepository<Role> roleRepository, IMapper mapper,
            IConfiguration configuration, CryptoService cryptoService, EmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _configuration = configuration;
            _cryptoService = cryptoService;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn([FromBody] UserLogInModel userLogIn)
        {
            var user = await _userRepository.FindByAsync(userLogIn.NicknameOrEmail);

            if (user == null)
            {
                return NotFound(new { errorText = "User does not exist." });
            }

            if (_cryptoService.Encrypt(userLogIn.Password) != user.Password)
            {
                return BadRequest(new { errorText = "Wrong password." });
            }

            var claimsIdentity = await GetIdentityAsync(user);

            if (claimsIdentity == null)
            {
                return BadRequest(new { errorText = "Couldn't get user identity." });
            }

            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);            

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrinciple);

            return Ok(new { nickname = user.Nickname, roles = claimsIdentity.RoleClaimType.Split(rolesSeperator) });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex) 
            {
                return Problem($"Logout failed. {ex.Message}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegister)
        {
            if (userRegister.Password == null)
            {
                return BadRequest("Password is null");
            }

            if (userRegister.Email == null)
            {
                return BadRequest("Email is null");
            }

            if (await DoesUserExist(_cryptoService.Encrypt(userRegister.Email)))
            {
                return BadRequest(new { errorText = "User with this email already exists." });
            }

            var url = $"<button>" +
                $"<a href='{_configuration["ClientUrl"]}/api/authorization/confirm?" +
                $"encryptedEmail={_cryptoService.Encrypt(userRegister.Email)}&encryptedPassword={_cryptoService.Encrypt(userRegister.Password)}" +
                $"&firstName={userRegister.FirstName}&lastName={userRegister.LastName}' " +
                $"style=\"text-decoration: none; color: black\">" +
                $"Confirm Registration" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(userRegister.Email, "Please, confirm Your registration", url);

            if (result?.Value.Status == EmailSendStatus.Succeeded)
            {
                return Ok(new { message = "Email sent" });
            }
            else
            {
                return BadRequest("Error while sending email");
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Confirm([FromQuery] string encryptedEmail, [FromQuery] string encryptedPassword, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            var user = await _userRepository.CreateAsync(new User { Email = encryptedEmail, Password = encryptedPassword, FirstName = firstName, LastName = lastName });

            if (user == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=User%20could%20not%20be%20created.");
            }

            var result = await LogIn(new UserLogInModel { NicknameOrEmail = _cryptoService.Decrypt(user.Email), Password = _cryptoService.Decrypt(user.Password) });

            var okResult = result as OkObjectResult;

            if (okResult == null || okResult.Value == null || okResult.Value as LogInResponseModel == null)
            {
                return Redirect($"{_configuration["ClientUrl"]}/confirm?success=false&message=Failed%20during%20login%20user%20{_cryptoService.Decrypt(encryptedEmail)}");
            }

            return Redirect($"{_configuration["ClientUrl"]}/confirm?key=");
        }

        private async Task<ClaimsIdentity?> GetIdentityAsync(User? user)
        {
            if (user != null)
            {
                var roles = await _roleRepository.GetListAsync(user.UserId);
                var roleType = string.Join(rolesSeperator, roles.Select(x => x?.RoleName));

                var claims = new List<Claim>
                    {
                        new (ClaimsIdentity.DefaultNameClaimType, user.Email ?? string.Empty),
                        new (ClaimsIdentity.DefaultRoleClaimType, roleType)
                    };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");                

                return claimsIdentity;
            }

            return null;
        }

        private async Task<bool> DoesUserExist(string? email)
        {
            return await _userRepository.FindByAsync(email) != null;
        }
    }
}
