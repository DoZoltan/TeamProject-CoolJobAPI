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
        Task<RefreshToken> GetByToken(string token);
        Task<bool> RemoveTokenById(int Id);
        Task<RefreshToken> GetPreviousToken(string userId);
    }
}
