using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoolJobAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

    }
}
