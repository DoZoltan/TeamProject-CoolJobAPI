using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        public DataType BirthDate { get; set; }
        public DataType RegistrationDate { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

    }
}
