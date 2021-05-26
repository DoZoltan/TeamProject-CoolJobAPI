using CoolJobAPI.Interfaces;
using CoolJobAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByUserName(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
