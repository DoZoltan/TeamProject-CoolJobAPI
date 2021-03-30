using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using CoolJobAPI.Models;

namespace CoolJobAPI.Controllers
{
    [EnableCors("Access-Control-Allow-Origin")]
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
            return _userRepository.AddNewUser(userName, password);
        }


        [HttpDelete("{userName}")]
        public IActionResult DeleteUser(string userName)
        {
            var user = _userRepository.DeleteUserByName(userName);

            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
