using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;

namespace Deloprosit.Bll.Services
{
    public class RoleRepository : IRepository<Role>
    {
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

        public Task<Role?> FindByAsync(string? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role?>> GetListAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> UpdateAsync(Role item)
        {
            throw new NotImplementedException();
        }
    }
}
