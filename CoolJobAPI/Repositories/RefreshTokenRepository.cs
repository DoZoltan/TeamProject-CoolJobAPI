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

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<RefreshToken> GetPreviousToken(string userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<bool> RemoveTokenById(int Id)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == Id);

            if (token == null)
            {
                return false;
            }

            try
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            };

            return true;
        }

        public async Task<bool> SaveToken(RefreshToken refreshToken)
        {
            try
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
