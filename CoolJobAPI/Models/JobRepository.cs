﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolJobAPI.Models
{
    public class JobRepository : IJobRepository
    {

        private readonly JobContext _context;

        public JobRepository(JobContext context)
        {
            _context = context;
        }

        public IEnumerable<Job> GetJobs()
        {
            return _context.Jobs.ToList();
        }

        public Job GetJobById(string jobId)
        {
            var a = jobId;
            return _context.Jobs.ToList().FirstOrDefault(job => job.Id == jobId);
        }

        // Get the jobs in a specific range
        public IEnumerable<Job> GetActualJobs(int pageNum)
        {
            return _context.Jobs.ToList().Where((job, i) => i < 10 * pageNum);
        }

        public void AddNewJob(Job job)
        {
            _context.Add(job);
            try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }
        }

        public Job DeleteJobById(string jobId)
        {
            var job = _context.Jobs.ToList().FirstOrDefault(job => job.Id == jobId);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                _context.SaveChanges();
            }
            return job;
        }

        public IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num)
        {
            string correctValue = filterValue.Replace("%20", " ");

            // Get the job object what have the specific property (variable) with the specific value
            var filtered = _context.Jobs.ToList().Where(job => job.GetType().GetProperty(filterBy).GetValue(job).ToString() == correctValue);
            return filtered.Where((job, i) => i < 10 * num);
        }

        public IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy)
        {
            // Get the unique filter values for the given filter type
            return _context.Jobs.ToList().Select(job => job.GetType().GetProperty(filterBy).GetValue(job).ToString());
        }

        // Get the favorites for the user
        public IEnumerable<Job> GetFavorites(int userId)
        {
            var favsByUser = _context.Favorites.Where(fav => fav.UserId == userId).ToArray();
            return favsByUser.Select(fav => _context.Jobs.FirstOrDefault(job => job.Id == fav.JobId));
        }

        // Add a job to the user's favorite list
        public void AddToFavorites(string jobId, int userId)
        {
            Favorite favorite = new Favorite();
            favorite.JobId = jobId;
            favorite.UserId = userId;
            if (_context.Favorites.Count() > 0)
            {
                favorite.FavId = ++_context.Favorites.Last().FavId;
            }
            else
            {
                favorite.FavId = 1;
            }

            _context.Favorites.Add(favorite);
            _context.SaveChanges();

            Console.WriteLine(_context.Favorites.FirstOrDefault().FavId);
            Console.WriteLine(_context.Favorites.FirstOrDefault().JobId);
            Console.WriteLine(_context.Favorites.FirstOrDefault().UserId);
        }

        public Favorite DeleteFavoriteJob(int favId)
        {
            var fav = _context.Favorites.FirstOrDefault(fav => fav.FavId == favId);

            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                _context.SaveChanges();
            }
            return fav;
        }

        public int GetFavId(string jobId, int userId)
        {
            var favorite = _context.Favorites.FirstOrDefault(fav => fav.JobId == jobId && fav.UserId == userId);

            if (favorite == null)
            {
                return -1;
            }

            return favorite.FavId;
        }
    }
}
