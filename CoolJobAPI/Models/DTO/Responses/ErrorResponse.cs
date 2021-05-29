using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models.DTO.Responses
{
    public class ErrorResponse
    {
        public IEnumerable<string> ErrorMessages { get; set; }

        public ErrorResponse(string massage)
        {
            ErrorMessages = new List<String>() { massage };
        }

        public ErrorResponse(IEnumerable<string> massages)
        {
            ErrorMessages = massages;
        }
    }
}
