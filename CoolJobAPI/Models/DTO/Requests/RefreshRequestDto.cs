using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models.DTO.Requests
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
