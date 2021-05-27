using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoolJobAPI.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
