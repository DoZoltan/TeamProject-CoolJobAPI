using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string RefreshSecret { get; set; }
    }
}
