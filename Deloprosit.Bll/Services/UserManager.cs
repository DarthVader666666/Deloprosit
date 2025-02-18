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

        public async Task<User?> GetUserByAsync(string? nicknameOrEmail, bool encrypted = false)
        {
            if (!encrypted)
            {
                nicknameOrEmail = _cryptoService.Encrypt(nicknameOrEmail);
            }

            return await _userRepository.FindByAsync(nicknameOrEmail);
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

            return (_cryptoService.Decrypt(user.Nickname), roles?.Split(rolesSeperator));
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

        public async Task CreateUser(User? user)
        {
            var url = $"<button>" +
                $"<a href='{_configuration["ClientUrl"]}/registration/confirm?" +
                $"key={user.Email}&encryptedPassword={user.Password}" +
                $"&firstName={user.FirstName}&lastName={user.LastName}' " +
                $"style=\"text-decoration: none; color: black\">" +
                $"Confirm Registration" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(user.Email, "Deloprosit: Письмо с подтверждением регистрации", url);
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
    }
}
