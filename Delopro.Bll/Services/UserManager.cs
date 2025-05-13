using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

using PasswordGenerator;

using System.Security.Claims;
using System.Text;

namespace Delopro.Bll.Services
{
    public class UserManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly CryptoService _cryptoService;
        private readonly IEmailSender _emailSender;

        private const char rolesSeperator = ',';
        private const string authorizationScheme = "Cookies";
        private const string key1 = "key1";
        private const string key2 = "key2";
        private const string and = "&amp;";

        public UserManager(IRepository<User> userRepository, IRepository<Role> roleRepository, CryptoService cryptoService, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cryptoService = cryptoService;
            _emailSender = emailSender;
        }

        public async Task<User?> GetCurrentUserAsync(HttpContext httpContext)
        {
            var nickname = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;

            return await GetUserByAsync(nickname);
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

            var roles = claimsIdentity.Claims.Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Select(x => x.Value).ToArray();

            return (user.Nickname, roles);
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
                $"<a href='{serverUrl}confirm?{key1}={nicknameByteString}{and}{key2}={emailByteString}'" +
                $"style=\"text-decoration: none; color: black\">" +
                $"Подтвердить регистрацию" +
                $"</a>" +
                $"</button>";

            var result = await Task.Run(() => _emailSender.SendEmail(user.Email ?? string.Empty, "Пожалуйста, подтвердите регистрацию в DeloPro", url));

            if (!result)
            {
                return false;
            }

            user.FirstName = _cryptoService.Encrypt(user.FirstName);
            user.LastName = _cryptoService.Encrypt(user.LastName);
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

        public string GeneratePassword()
        {
            var password = new Password()
                .IncludeLowercase()
                .IncludeUppercase()
                .IncludeNumeric()
                .IncludeSpecial()
                .LengthRequired(12);

            return password.Next();
        }

        public async Task ChangePasswordAsync(User? user, string password, bool doEncryptPassword = true)
        {
            if (user == null)
            { 
                throw new ArgumentNullException(nameof(user));
            }

            user.Password = doEncryptPassword ? _cryptoService.Encrypt(password) : password;
            await _userRepository.UpdateAsync(user);
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
                    };

                foreach (var role in rolesArray)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role?.RoleName ?? ""));
                }

                var claimsIdentity = new ClaimsIdentity(claims, authorizationScheme);

                return claimsIdentity;
            }

            return null;
        }

        private string? GetByteString(string? keyName, string? text, bool doEncrypt = true)
        {
            return string.Join($"{and}{keyName}=", Encoding.UTF8.GetBytes(doEncrypt ? _cryptoService.Encrypt(text) ?? "" : text ?? "").Select(x => x.ToString()));
        }        
    }
}
