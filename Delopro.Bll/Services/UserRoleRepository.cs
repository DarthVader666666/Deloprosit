using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Delopro.Bll.Services
{
    public class UserRoleRepository : IRepository<UserRole>
    {
        private readonly DeloproDbContext _dbContext;

        public UserRoleRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserRole?> CreateAsync(UserRole? item)
        {
            if (item == null)
            { 
                return null;
            }

            var userRole = _dbContext.UserRoles.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return userRole;
        }

        public async Task<UserRole?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var userRole = _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == id_1 && x.RoleId == id_2);
            var removedUserRole = await _dbContext.Remove(userRole).Entity;
            await _dbContext.SaveChangesAsync();

            return removedUserRole;
        }

        public async Task DeleteRangeAsync(IEnumerable<UserRole> items)
        {
            _dbContext.UserRoles.RemoveRange(items);
            await _dbContext.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(UserRole? item)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<UserRole?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserRole?>> GetListAsync(int? id = null)
        {
            var userRoles = _dbContext.UserRoles.Where(x => x.UserId == id);
            return Task.FromResult<IEnumerable<UserRole?>>(userRoles);
        }

        public Task<UserRole?> UpdateAsync(UserRole? item)
        {
            throw new NotImplementedException();
        }
    }
}
