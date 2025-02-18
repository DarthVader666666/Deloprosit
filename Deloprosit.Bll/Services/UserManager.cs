using Azure.Communication.Email;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using System.Security.Claims;

namespace Deloprosit.Bll.Services
{
    public class UserManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly CryptoService _cryptoService;
        private readonly EmailSender _emailSender;

        private const char rolesSeperator = ',';
        private const string authorizationScheme = "Cookies";

        public UserManager(IRepository<User> userRepository, IRepository<Role> roleRepository, IConfiguration configuration, CryptoService cryptoService, EmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cryptoService = cryptoService;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<User?> GetUserByAsync(string? nickname = null, string? email = null)
        {
            if (nickname == null)
            {
                return await _userRepository.FindByAsync(_cryptoService.Encrypt(email));
            }
            else
            {
                return await _userRepository.FindByAsync(nickname);
            }
        }

        public bool IsMatchPassword(User user, string? encryptedPassword)
        {
            return _cryptoService.Decrypt(user.Password) == encryptedPassword;
        }

        public async Task<(string? Nickname, string[]? Roles)?> LogIn(User user, HttpContext httpContext)
        {
            var claimsIdentity = await GetIdentityAsync(user);

            if (claimsIdentity == null)
            {
                return null;
            }

            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await httpContext.SignInAsync(authorizationScheme, claimsPrinciple);

            var roles = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;

            return (user.Nickname, roles?.Split(rolesSeperator));
        }

        public async Task LogOut(HttpContext httpContext)
        {
            try
            {
                await httpContext.SignOutAsync(authorizationScheme);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RegisterAsync(User? user)
        {
            if (user == null)
            {
                return false;
            }

            var email = user.Email;
            user.Email = _cryptoService.Encrypt(email);
            user.Password = _cryptoService.Encrypt(user.Password);

            var createdUser = await _userRepository.CreateAsync(user);

            if (createdUser == null || createdUser.Email == null)
            {
                return false;
            }

            var url = 
                $"<button>" +
                $"<a href='{_configuration["ClientUrl"]}/registration/confirm?key={createdUser.Email}'" +
                $"style=\"text-decoration: none; color: black\">" +
                $"Подтвердить регистрацию" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(email, "Пожалуйста, подтвердите регистрацию", url);

            return result;
        }

        private async Task<ClaimsIdentity?> GetIdentityAsync(User? user)
        {
            if (user != null)
            {
                var roles = await _roleRepository.GetListAsync(user.UserId);
                var roleType = string.Join(rolesSeperator, roles.Select(x => x?.RoleName));

                var claims = new List<Claim>
                    {
                        new (ClaimsIdentity.DefaultNameClaimType, _cryptoService.Decrypt(user.Email) ?? string.Empty),
                        new (ClaimsIdentity.DefaultRoleClaimType, roleType)
                    };

                var claimsIdentity = new ClaimsIdentity(claims, authorizationScheme);

                return claimsIdentity;
            }

            return null;
        }

        public async Task<bool> DoesUserExistAsync(string? nicknameOrEmail, bool encrypted = false)
        {
            if (!encrypted)
            {
                nicknameOrEmail = _cryptoService.Encrypt(nicknameOrEmail);
            }

            return await _userRepository.FindByAsync(nicknameOrEmail) != null;
        }

        //public async Task<string> DecodeUnicodeArray(int[]? charCodes)
        //{ 
            
        //}
    }
}
