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
        public int FavId { get; set; }
        public string JobId { get; set; }
        public int UserId { get; set; }
    }
}
