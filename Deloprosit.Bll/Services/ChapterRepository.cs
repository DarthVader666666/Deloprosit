﻿using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;

using Microsoft.EntityFrameworkCore;

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

            var createdChapter = _dbContext.Chapters.Add(item).Entity;
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
            return Task.FromResult(_dbContext.Chapters.Include(x => x.Themes).FirstOrDefault(x => x.ChapterId == id));
        }

        public Task<IEnumerable<Chapter?>> GetListAsync(int? id = null)
        {
            return Task.FromResult<IEnumerable<Chapter?>>(_dbContext.Chapters.AsEnumerable());
        }

        public async Task<Chapter?> UpdateAsync(Chapter? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedChapter = _dbContext.Chapters.Update(item).Entity;
            await _dbContext.SaveChangesAsync();

            return updatedChapter;
        }
    }
}
