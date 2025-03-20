using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;

using Microsoft.Data.SqlClient;

namespace Deloprosit.Bll.Services
{
    public class MessageRepository: IRepository<Message>
    {
        private readonly DeloprositDbContext _dbContext;

        public MessageRepository(DeloprositDbContext dbContext)
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

        public Task<Message?> DeleteAsync(int? id)
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
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message?>> GetListAsync(int? id = null)
        {
            if (id == null)
            {
                return Task.FromResult<IEnumerable<Message?>>([]);
            }

            IEnumerable<Message?> messages = [];

            try 
            {
                messages = _dbContext.Messages.Where(x => x.UserId == id).OrderByDescending(x => x.DateSent);
            }
            catch(SqlException)
            {
                return Task.FromResult<IEnumerable<Message?>>([]);
            }


            return Task.FromResult(messages);
        }

        public Task<Message?> UpdateAsync(Message? item)
        {
            throw new NotImplementedException();
        }
    }
}
