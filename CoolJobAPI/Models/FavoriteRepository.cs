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
        public IEnumerable<Job> GetFavorites(int userId)
        {
            return _context.Favorites.Where(fav => fav.User.Id == userId).Select(fav => fav.Job);
        }
        
        public Job GetFavoriteJob(int jobId, int userId)
        {
            return GetFavorites(userId).FirstOrDefault(job => job.Id == jobId);
        }

        // Add a job to the user's favorite list
        public bool AddToFavorites(int jobId, int userId)
        {
            Favorite favorite = new Favorite();
            favorite.Job = _context.Jobs.FirstOrDefault(job => job.Id == jobId);
            favorite.User = _context.Users.FirstOrDefault(user => user.Id == userId);

            bool addWasSuccessfull = true;

            if (favorite.Job != null && favorite.User != null)
            {
                try
                {
                    _context.Favorites.Add(favorite);
                    _context.SaveChanges();
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

        public Favorite DeleteFavoriteJob(int favId)
        {
            var fav = _context.Favorites.FirstOrDefault(fav => fav.Id == favId);

            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                _context.SaveChanges();
            }
            return fav;
        }

        public int GetFavId(int jobId, int userId)
        {
            var favorite = _context.Favorites.FirstOrDefault(fav => fav.Job.Id == jobId && fav.User.Id == userId);

            if (favorite == null)
            {
                return -1;
            }

            return favorite.Id;
        }
    }
}
