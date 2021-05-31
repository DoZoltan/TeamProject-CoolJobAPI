using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models.DTO.Responses
{
    public class SuccessfulAuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
