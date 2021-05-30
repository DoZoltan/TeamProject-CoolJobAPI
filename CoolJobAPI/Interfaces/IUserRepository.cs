using CoolJobAPI.Models;
using CoolJobAPI.Models.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUserName(string userName);
        Task<User> GetById(string Id);
        Task<User> CreateUser(UserRegistrationRequestDto user);
        Task<bool> CheckThePassword(User user, string password);
        Task<bool> DeleteUser(User user);
    }
}
