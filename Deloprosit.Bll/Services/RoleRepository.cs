﻿using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace Deloprosit.Bll.Services
{
    public class RoleRepository : IRepository<Role>
    {
        public readonly DeloprositDbContext _dbContext;

        public RoleRepository(DeloprositDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Role?> CreateAsync(Role item)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> DeleteAsync(int? id)
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

        public Task<IEnumerable<Role?>> GetListAsync(int? id)
        {
            return Task.FromResult<IEnumerable<Role?>>(_dbContext.AccountRoles.Where(x => x.AccountId == id)
                .SelectMany(userRole => _dbContext.Roles.Where(role => userRole.RoleId == role.RoleId)));
        }

        public Task<Role?> UpdateAsync(Role item)
        {
            throw new NotImplementedException();
        }
    }
}
