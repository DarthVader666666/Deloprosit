﻿using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

namespace Delopro.Bll.Services
{
    public class RoleRepository : IRepository<Role>
    {
        public readonly DeloproDbContext _dbContext;

        public RoleRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Role?> CreateAsync(Role item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Role> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Role? item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role?>> GetListAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Role?>>(_dbContext.UserRoles.Where(x => x.UserId == id)
                .SelectMany(userRole => _dbContext.Roles.Where(role => userRole.RoleId == role.RoleId)));
        }

        public Task<Role?> UpdateAsync(Role item)
        {
            throw new NotImplementedException();
        }
    }
}
