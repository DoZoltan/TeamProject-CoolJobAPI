﻿using System;
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
            return from fav in _context.Favorites
                    where fav.User.Id == userId
                    select fav.Job;
        }

        // Add a job to the user's favorite list
        public void AddToFavorites(string jobId, int userId)
        {
            Favorite favorite = new Favorite();
            favorite.Job = _context.Jobs.Where(job => job.Id == jobId).ToArray()[0];
            favorite.User = _context.Users.Where(user => user.Id == userId).ToArray()[0];

            _context.Favorites.Add(favorite);
            _context.SaveChanges();
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

        public int GetFavId(string jobId, int userId)
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
