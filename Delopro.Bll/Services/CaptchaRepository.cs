using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

namespace Delopro.Bll.Services
{
    public class CaptchaRepository : IRepository<Captcha>
    {
        private readonly DeloproDbContext _dbContext;

        public CaptchaRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Captcha?> CreateAsync(Captcha? item)
        {
            throw new NotImplementedException();
        }

        public Task<Captcha?> DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Captcha? item)
        {
            throw new NotImplementedException();
        }

        public Task<Captcha?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Captcha?> GetAsync(int? id)
        {
            var random = new Random();

            var query = _dbContext.Captchas;
            var captcha = query.ElementAt(random.Next(0, query.Count()));

            return Task.FromResult<Captcha?>(captcha);
        }

        public Task<IEnumerable<Captcha?>> GetListAsync(int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task<Captcha?> UpdateAsync(Captcha? item)
        {
            throw new NotImplementedException();
        }
    }
}
