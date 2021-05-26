using CoolJobAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Interfaces
{
    interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUserName(string userName);
    }
}
