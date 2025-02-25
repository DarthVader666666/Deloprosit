using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;

namespace Deloprosit.Bll.Services
{
    public class ChapterRepository : IRepository<Chapter>
    {
        private readonly DeloprositDbContext _dbContext;

        public ChapterRepository(DeloprositDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Chapter?> CreateAsync(Chapter? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdChapter = _dbContext.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdChapter;
        }

        public Task<Chapter?> DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Chapter? item)
        {
            throw new NotImplementedException();
        }

        public Task<Chapter?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Chapter?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Chapter?>> GetListAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Chapter?>>(_dbContext.Chapters.AsEnumerable());
        }

        public Task<Chapter?> UpdateAsync(Chapter? item)
        {
            throw new NotImplementedException();
        }
    }
}
