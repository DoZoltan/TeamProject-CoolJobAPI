using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly JobContext _context;

        public FavoriteRepository(JobContext context)
        {
            _context = context;
        }

        // Get the favorites for the user
        public async Task<IEnumerable<Job>> GetFavorites(int userId)
        {
            return await _context.Favorites.Where(fav => fav.User.Id == userId).Select(fav => fav.Job).ToListAsync();
        }
        
        public async Task<Job> GetFavoriteJob(int jobId, int userId)
        {
            var favorites = await GetFavorites(userId);
            return favorites.FirstOrDefault(job => job.Id == jobId);
        }

        // Add a job to the user's favorite list
        public async Task<bool> AddToFavorites(int jobId, int userId)
        {
            Favorite favorite = new Favorite();
            favorite.Job = await _context.Jobs.FirstOrDefaultAsync(job => job.Id == jobId);
            favorite.User = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            bool addWasSuccessfull = true;

            if (favorite.Job != null && favorite.User != null)
            {
                try
                {
                    await _context.Favorites.AddAsync(favorite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    addWasSuccessfull = false;
                }
            }
            else 
            {
                addWasSuccessfull = false;
            }

            return addWasSuccessfull;
        }

        public async Task<Favorite> DeleteFavoriteJob(int favId)
        {
            var fav = await _context.Favorites.FirstOrDefaultAsync(fav => fav.Id == favId);

            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
            }
            return fav;
        }

        public async Task<int> GetFavId(int jobId, int userId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(fav => fav.Job.Id == jobId && fav.User.Id == userId);

            if (favorite == null)
            {
                return -1;
            }

            return favorite.Id;
        }
    }
}
