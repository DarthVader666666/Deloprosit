using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Deloprosit.Bll.Services
{
    public class UserManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly CryptoService _cryptoService;
        private const char rolesSeperator = ',';
        private const string authorizationScheme = "Cookies";

        public UserManager(IRepository<User> userRepository, IRepository<Role> roleRepository, CryptoService cryptoService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _cryptoService = cryptoService;
        }

        public async Task<User?> GetUserByAsync(string? nicknameOrEmail)
        { 
            return await _userRepository.FindByAsync(nicknameOrEmail);
        }

        public bool IsMatchPassword(User user, string? encryptedPassword)
        {
            return _cryptoService.Encrypt(user.Password) == encryptedPassword;
        }

        public async Task<string[]?> LogIn(User user, HttpContext httpContext)
        {
            var claimsIdentity = await GetIdentityAsync(user);

            if (claimsIdentity == null)
            {
                return null;
            }

            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await httpContext.SignInAsync(authorizationScheme, claimsPrinciple);

            var roles = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;

            return roles?.Split(rolesSeperator);
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
    }
}
