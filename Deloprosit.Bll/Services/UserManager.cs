using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;

namespace Deloprosit.Bll.Services
{
    public class UserManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly CryptoService _cryptoService;

        public UserManager(IRepository<User> userRepository, CryptoService cryptoService)
        {
            _userRepository = userRepository;
            _cryptoService = cryptoService;
        }

        public async Task<User?> GetUserByAsync(string? nicknameOrEmail)
        { 
            return await _userRepository.FindByAsync(nicknameOrEmail);
        }

        //public async Task<bool> DoesPasswordMatch(User user, string encryptedPassword)
        //{ 
            
        //}
    }
}
