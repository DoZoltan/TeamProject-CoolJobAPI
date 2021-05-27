using CoolJobAPI.Interfaces;
using CoolJobAPI.Models;
using CoolJobAPI.Models.DTO.Requests;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly JobContext _context;
        private readonly UserManager<User> _userManager;


        public UserRepository(JobContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User> CreateUser(UserRegistrationRequestDto user)
        {
            var newUser = new User()
            {
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture,
                BirthDate = user.BirthDate,
                RegistrationDate = DateTime.Now
            };

            var isCreated = await _userManager.CreateAsync(newUser, user.Password);
            
            if (isCreated.Succeeded)
            {
                return newUser;
            }

            return null;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
    }
}
