﻿using Azure.Communication.Email;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Data;
using Deloprosit.Data.Entities;
using System.Net.Mail;

namespace Deloprosit.Bll.Services
{
    public class UserRepository : IRepository<User>
    {
        private readonly DeloprositDbContext _dbContext;

        public UserRepository(DeloprositDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User?> CreateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> DeleteAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(User? item)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindByAsync(object? parameter)
        {
            if (parameter is not string)
            {
                return Task.FromResult<User?>(null);
            }

            User? user = null;

            if (IsValidEmail(parameter as string))
            {
                user = _dbContext.Users.FirstOrDefault(user => user.Email == (parameter as string));
            }
            else
            {
                user = _dbContext.Users.FirstOrDefault(user => user.Nickname == parameter as string);
            }

            return Task.FromResult(user);
        }

        public Task<User?> GetAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User?>> GetListAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }

        private static bool IsValidEmail(string? email)
        {
            if (email == null)
            { 
                return false;
            }

            var beforeAt = email.Split('@');

            if (beforeAt.Length != 2)
            {
                return false;
            }

            var afterAt = beforeAt[1].Split('.');

            if (afterAt.Length != 2)
            {
                return false;
            }

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
