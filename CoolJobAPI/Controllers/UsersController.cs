using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using CoolJobAPI.Models;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> PostNewUser(User newUser)
        {
          //  return Conflict();
            return await _userRepository.RegisterNewUser(newUser);
        }


        [HttpDelete("{userName}")]
        public IActionResult DeleteUser(string userName)
        {
            var isUserExits = _userRepository.DeleteUserByName(userName);

            if (!isUserExits)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
