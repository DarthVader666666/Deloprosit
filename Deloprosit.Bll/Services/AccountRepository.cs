using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;

namespace Deloprosit.Bll.Services
{
    public class AccountRepository : IRepository<Account>
    {
        public Task<Account?> CreateAsync(Account item)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Account? item)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Account?>> GetListAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<Account?> UpdateAsync(Account item)
        {
            throw new NotImplementedException();
        }
    }
}
