using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deloprosit.Bll.Services
{
    public class UserRepository : IRepository<User>
    {
        private readonly DeloprositDbContext _dbContext;

        public UserRepository(DeloprositDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> CreateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(User? item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByAsync(object? parameter)
        {
            if (parameter is not string)
            {
                return Task.FromResult<User?>(null);
            }

            var account = _dbContext.Users.FirstOrDefault(account => account.Nickname == parameter as string);

            return Task.FromResult(account);
        }

        public Task<User?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User?>> GetListAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }
    }
}
