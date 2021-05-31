using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using CoolJobAPI.Interfaces;

namespace CoolJobAPI.Models
{
    public class JobRepository : IJobRepository
    {
        private readonly JobContext _context;

        public JobRepository(JobContext context)
        {
            _context = context;
        }


        public async Task<bool> ClearDB()
        {
            bool clearWasSuccessful = true;

            try
            {
                foreach (var token in _context.RefreshTokens)
                    _context.RefreshTokens.Remove(token);
                foreach (var entity in _context.Jobs)
                    _context.Jobs.Remove(entity);
                foreach (var entity in _context.Users)
                    if(entity.UserName != "Admin")
                        _context.Users.Remove(entity);
                foreach (var entity in _context.Favorites)
                    _context.Favorites.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                clearWasSuccessful = false;
            }

            return clearWasSuccessful;
        }

        public async Task<string> GetAdminKey()
        {
            Dictionary<string, string> adminKey;

            try
            {
                using (StreamReader r = new StreamReader("wwwroot/data/admin.json"))
                {
                    string json = await r.ReadToEndAsync();
                    adminKey = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }

                return adminKey["adminKey"];
            }
            catch (IOException)
            {
                return "Some IO error happened!";
            }
        }

        public async Task<bool> LoadJson(string userId)
        {
            bool loadWasSuccessful = true;

            List<Job> jobs = new List<Job>();

            try
            {
                //only for test
                using (StreamReader r = new StreamReader("wwwroot/data/data.json"))
                {
                    string json = await r.ReadToEndAsync();
                    jobs = JsonConvert.DeserializeObject<List<Job>>(json);
                }
            }
            catch (IOException)
            {
                loadWasSuccessful = false;
            }

            try
            {

                foreach (var job in jobs)
                {
                    //job.User = user;
                    await AddNewJob(job, userId);

                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                loadWasSuccessful = false;
            }

            return loadWasSuccessful;

        }

        public async Task<IEnumerable<Job>> GetJobs()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<int> GetNumberOfTheJobs()
        {
            return await _context.Jobs.CountAsync();
        }

        public async Task<Job> GetJobById(int jobId)
        {
            return await _context.Jobs.FirstOrDefaultAsync(job => job.Id == jobId);
        }

        // Get the jobs in a specific range
        public async Task<IEnumerable<Job>> GetJobsByRange(int pageNum)
        {
            int correctPageNum = pageNum < 1 ? 1 : pageNum;

            return await _context.Jobs.Take(correctPageNum * 10).ToListAsync();    //AsAsyncEnumerable();
        }

        public async Task<Job> AddNewJob(Job job, string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            job.User = user;
            
            try
            {
                await _context.AddAsync(job);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return null;
            }

            var lastId = await _context.Jobs.MaxAsync(job => job.Id);
            return await GetJobById(lastId);
        }

        public async Task<bool> DeleteJobById(int jobId)
        {
            // Is the job exists what I want to delete?
            var job = await _context.Jobs.FirstOrDefaultAsync(job => job.Id == jobId);

            bool deleteWasSuccessfull = true;

            // Try to delete is if yes
            if (job != null)
            {
                try
                {
                    _context.Jobs.Remove(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    deleteWasSuccessfull = false;
                }

                return deleteWasSuccessfull;
                
            }
            
            return false;
        }

        // How to handle if the object/model/entity value is Null? (if it null then the .GetType() method cause NullReferenceException)
        public IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int pageNum)
        {
            // Make the filterBy (property name) case insensitive (convert the 1st char to upper case and to lover case the others)
            string correctType = filterBy.Length > 1 ? filterBy[0].ToString().ToUpper() + filterBy[1..filterBy.Length].ToLower() : filterBy;

            // Replace the %20 (spaces from the URL) to spaces, and make it case insensitive
            string correctValue = filterValue.Replace("%20", " ").ToLower();

            int correctPageNum = pageNum < 1 ? 1 : pageNum;

            // Get the jobs what have the provided property (filter type)
            var jobsWithSpecificProperties = _context.Jobs.AsEnumerable().Where(job => job.GetType().GetProperty(correctType) != null);

            // Get the jobs what have the specific property with the specific value
            var filtered = jobsWithSpecificProperties.Where(job => job.GetType().GetProperty(correctType).GetValue(job).ToString().ToLower().Contains(correctValue));

            return filtered.Take(correctPageNum * 10);
        }

        public IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy)
        {
            // Make the filterBy (property name) case insensitive (convert the 1st char to upper case and to lover case the others)
            string correctType = filterBy.Length > 1 ? filterBy[0].ToString().ToUpper() + filterBy[1..filterBy.Length].ToLower() : filterBy;

            // Get the unique filter values for the given filter type
            return _context.Jobs.AsEnumerable().Select(job => job.GetType().GetProperty(correctType)?.GetValue(job).ToString()).Distinct();
        }
    }
}
