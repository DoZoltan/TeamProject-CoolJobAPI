using CoolJobAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Interfaces
{
    interface IRefreshTokenRepository
    {
        Task<string> Create(RefreshToken refreshtoken);
    }
}
