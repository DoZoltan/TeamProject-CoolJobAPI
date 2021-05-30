using CoolJobAPI.Interfaces;
using CoolJobAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        JobContext _context;


        public RefreshTokenRepository(JobContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveToken(RefreshToken refreshToken)
        {
            try
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
