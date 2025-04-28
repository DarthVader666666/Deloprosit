using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

namespace Delopro.Bll.Services
{
    public class UserRepository : IRepository<User>
    {
        private readonly DeloprositDbContext _dbContext;

        public UserRepository(DeloprositDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> CreateAsync(User? item)
        {
            if (item == null)
            { 
                return null;
            }

            var createdUser = _dbContext.Users.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            _dbContext.UserRoles.Add(new UserRole { UserId = createdUser.UserId });
            await _dbContext.SaveChangesAsync();

            return createdUser;
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

            var user = _dbContext.Users.FirstOrDefault(user => user.Email == parameter as string);
            user ??= _dbContext.Users.FirstOrDefault(user => user.Nickname == parameter as string);

            return Task.FromResult(user);
        }

        public Task<User?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User?>> GetListAsync(int? id = null)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> UpdateAsync(User? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedUser = _dbContext.Users.Update(item).Entity;
            int result = await _dbContext.SaveChangesAsync();

            if (result <= 0)
            {
                return null;
            }

            return updatedUser;
        }
    }
}
