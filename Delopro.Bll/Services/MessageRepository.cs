﻿using Delopro.Bll.Interfaces;
using Delopro.Data;
using Delopro.Data.Entities;

using Microsoft.Data.SqlClient;

namespace Delopro.Bll.Services
{
    public class MessageRepository: IRepository<Message>
    {
        private readonly DeloproDbContext _dbContext;

        public MessageRepository(DeloproDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Message?> CreateAsync(Message? item)
        {
            if (item == null)
            { 
                return null;
            }

            var message = await _dbContext.Messages.AddAsync(item);
            _dbContext.SaveChanges();

            return message.Entity;
        }

        public Task<Message?> DeleteAsync(int? id_1, int? id_2 = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRangeAsync(IEnumerable<Message> items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Message? item)
        {
            throw new NotImplementedException();
        }

        public Task<Message?> FindByAsync(object? parameter)
        {
            throw new NotImplementedException();
        }

        public Task<Message?> GetAsync(int? id)
        {
            if (id == null)
            {
                return Task.FromResult<Message?>(null);
            }

            Message? message;

            try
            {
                message = _dbContext.Messages.FirstOrDefault(x => id == x.MessageId);
            }
            catch (SqlException)
            {
                return Task.FromResult<Message?>(null);
            }


            return Task.FromResult(message);
        }

        public Task<IEnumerable<Message?>> GetListAsync(int? id = null)
        {
            IEnumerable<Message?> messages = [];

            try
            {
                messages = id != null ? _dbContext.Messages.Where(x => x.UserId == id).OrderByDescending(x => x.DateSent) : _dbContext.Messages.OrderByDescending(x => x.DateSent);
            }
            catch(SqlException)
            {
                return Task.FromResult<IEnumerable<Message?>>([]);
            }

            return Task.FromResult(messages);
        }

        public async Task<Message?> UpdateAsync(Message? item)
        {
            if (item == null)
            {
                return null;
            }

            var updatedMessage = _dbContext.Messages.Update(item).Entity;
            await _dbContext.SaveChangesAsync();

            return updatedMessage;
        }
    }
}
