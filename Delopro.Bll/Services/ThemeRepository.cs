using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

using Microsoft.EntityFrameworkCore;

namespace Delopro.Bll.Services
{
    public class ThemeRepository: IRepository<Theme>
    {
        private readonly DeloproDbContext _dbContext;

        public ThemeRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Theme?> CreateAsync(Theme? item)
        {
            if (item == null)
            {
                return null;
            }

            var createdTheme = _dbContext.Themes.Add(item).Entity;
            await _dbContext.SaveChangesAsync();

            return createdTheme;
        }

        public async Task<Theme?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            var theme = await GetAsync(id_1);

            if (theme == null)
            {
                return null;
            }

            var deletedTheme = _dbContext.Themes.Remove(theme).Entity;
            await _dbContext.SaveChangesAsync();

            return deletedTheme;
        }

        public Task DeleteRangeAsync(IEnumerable<Theme> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Theme? item)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Theme?> GetAsync(int? id)
        {
            return Task.FromResult(_dbContext.Themes.FirstOrDefault(x => x.ThemeId == id));
        }

        public Task<IEnumerable<Theme?>> GetListAsync(int? id = null)
        {
            var themes = id == null ? _dbContext.Themes : _dbContext.Chapters.Include(x => x.Themes).FirstOrDefault(x => x.ChapterId == id)?.Themes?.AsEnumerable<Theme?>();

            return Task.FromResult(themes ?? []);
        }

        public async Task<Theme?> UpdateAsync(Theme? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedTheme = _dbContext.Themes.Update(item).Entity;
            await _dbContext.SaveChangesAsync();

            return updatedTheme;
        }
    }
}
