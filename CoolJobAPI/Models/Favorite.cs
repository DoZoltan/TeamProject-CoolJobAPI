using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }
        public Job Job { get; set; }
        public User User { get; set; }
    }
}
