using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly JobContext _context;
        private UserManager<User> userManager;

        public UserRepository(JobContext context)
        {
            _context = context;
        }

        public bool DeleteUserByName(string userName)
        {
            /*
            var user = IsUserExists(userName);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            */
            return false;
            
        }

        public async Task<bool> RegisterNewUser(User newUser)
        {
            var user = await userManager.FindByEmailAsync(newUser.Email);

            /*
            try
            {
                var user = IsUserExists(userName);
                if (user != null)
                {
                    _context.Users.Add(user);
                }
                else
                {
                    return false;
                }
                _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
            */

            return true;
        }
    }
}
