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
    public class UsersController : Controller
    {
        IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public ActionResult<bool> PostNewUser(string userName, string password)
        {
          //  return Conflict();
            return _userRepository.AddNewUser(userName, password); // better with status code etc: 201 or 409 conflict if exist
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
