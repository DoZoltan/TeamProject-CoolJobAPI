using CoolJobAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> SaveToken(RefreshToken refreshtoken);
    }
}
