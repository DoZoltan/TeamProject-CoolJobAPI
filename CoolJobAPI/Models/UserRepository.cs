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

        public UserRepository(JobContext context)
        {
            _context = context;
        }

        public bool DeleteUserByName(string userName)
        {
            var user = IsUserExists(userName);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddNewUser(string userName, string password)
        {

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

            return true;
        }

        private User IsUserExists(string userName)
        {
            return _context.Users.ToList().FirstOrDefault(user => user.UserName == userName);
        }
    }
}
