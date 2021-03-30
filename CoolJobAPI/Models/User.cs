using System.ComponentModel.DataAnnotations;

namespace CoolJobAPI.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
