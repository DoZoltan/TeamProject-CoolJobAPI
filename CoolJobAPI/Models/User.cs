using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoolJobAPI.Models
{
    public class User : IdentityUser
    {
        [Key]
        public new int Id { get; set; }  //--> in IdentityUser
        //public string UserName { get; set; }  //--> in IdentityUser
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Email { get; set; }  //--> in IdentityUser
        public string ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        //public string Password { get; set; }  //--> in IdentityUser
        //public string PasswordSalt { get; set; }  //--> in IdentityUser
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
