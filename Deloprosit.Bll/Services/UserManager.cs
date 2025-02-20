using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

using System.Security.Claims;
using System.Text;

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
        private const string key1 = "key1";
        private const string key2 = "key2";

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

        public async Task<bool> RegisterAsync(User? user, string? serverUrl)
        {
            if (user == null)
            {
                return false;
            }

            var nicknameByteString = GetByteString(key1, user.Nickname);
            var emailByteString = GetByteString(key2, user.Email);

            var url = 
                $"<button type=\"button\" style=\"border: black; border-width: 1px\">" +
                $"<a href='{serverUrl}confirm?{key1}={nicknameByteString}&amp;{key2}={emailByteString}'" +
                $"style=\"text-decoration: none; color: black\">" +
                $"Подтвердить регистрацию" +
                $"</a>" +
                $"</button>";

            var result = await _emailSender.SendEmailAsync(user.Email ?? string.Empty, "Пожалуйста, подтвердите регистрацию в Deloprosit", url);

            if (!result)
            {
                return false;
            }

            user.Email = _cryptoService.Encrypt(user.Email);
            user.Password = _cryptoService.Encrypt(user.Password);

            var createdUser = await _userRepository.CreateAsync(user);

            if (createdUser == null)
            {
                return false;
            }

            return result;
        }

        public async Task<User?> ConfirmUserAsync(string[]? keys)
        {
            var encryptedNickname = keys?[0];
            var encryptedEmail = keys?[1];

            var user = await GetUserByAsync(_cryptoService.Decrypt(encryptedNickname));

            if (user == null || user.Email != encryptedEmail || user.IsConfirmed)
            {
                return null;
            }

            user.IsConfirmed = true;

            var updatedUser = await _userRepository.UpdateAsync(user);

            if (updatedUser == null)
            {
                return null;
            }

            return updatedUser;
        }

        public async Task<bool> DoesUserExistAsync(string? nicknameOrEmail, bool doEncrypt = false)
        {
            if (doEncrypt)
            {
                nicknameOrEmail = _cryptoService.Encrypt(nicknameOrEmail);
            }

            return await _userRepository.FindByAsync(nicknameOrEmail) != null;
        }

        private async Task<ClaimsIdentity?> GetIdentityAsync(User? user)
        {
            if (user != null)
            {
                var rolesArray = await _roleRepository.GetListAsync(user.UserId);

                var claims = new List<Claim>
                    {
                        new (ClaimsIdentity.DefaultNameClaimType, user.Nickname ?? string.Empty),
                        new ("Email", _cryptoService.Decrypt(user.Email) ?? string.Empty),
                        new (ClaimsIdentity.DefaultRoleClaimType, string.Join(rolesSeperator, rolesArray.Select(x => x?.RoleName)))
                    };

                var claimsIdentity = new ClaimsIdentity(claims, authorizationScheme);

                return claimsIdentity;
            }

            return null;
        }

        private string? GetByteString(string? keyName, string? text, bool doEncrypt = true)
        {
            return string.Join($"&amp;{keyName}=", Encoding.UTF8.GetBytes(doEncrypt ? _cryptoService.Encrypt(text) ?? "" : text ?? "").Select(x => x.ToString()));
        }


        //private static bool IsValidEmail(string? email)
        //{
        //    if (email == null)
        //    { 
        //        return false;
        //    }

        //    var beforeAt = email.Split('@');

        //    if (beforeAt.Length != 2)
        //    {
        //        return false;
        //    }

        //    var afterAt = beforeAt[1].Split('.');

        //    if (afterAt.Length != 2)
        //    {
        //        return false;
        //    }

        //    try
        //    {
        //        var addr = new MailAddress(email);
        //        return addr.Address == email;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
