using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace CoolJobAPI.Models
{
    public class JobRepository : IJobRepository
    {
        private readonly JobContext _context;

        public JobRepository(JobContext context)
        {
            _context = context;
        }

        public void ClearDB()
        {
            foreach (var entity in _context.Jobs)
                _context.Jobs.Remove(entity);
            foreach (var entity in _context.Users)
                _context.Users.Remove(entity);
            foreach (var entity in _context.Favorites)
                _context.Favorites.Remove(entity);
            _context.SaveChanges();     
        }

        public void LoadJson()
        {
            //only for test
            List<Job> jobs;
            using (StreamReader r = new StreamReader("wwwroot/data/data.json"))
            {
                string json = r.ReadToEnd();
                jobs = JsonConvert.DeserializeObject<List<Job>>(json);
            }
            User user = new User
            {
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "Admin",
                Email = "kumkvatmailcool@gmail.com",
                ProfilePicture = "picture",
                BirthDate = DateTime.Now,
                RegistrationDate = DateTime.Now,
                Password = "admin1234",
                PasswordSalt = "sugar",
            }; // just for try to use user for jobs

        _context.Add(user);

            foreach (var job in jobs)
            {
                job.User = user;
                AddNewJob(job);
                
            }
            _context.SaveChanges();

        }

        public IEnumerable<Job> GetJobs()
        {
            return _context.Jobs;
        }

        public Job GetJobById(int jobId)
        {
            return _context.Jobs.FirstOrDefault(job => job.Id == jobId);
        }

        // Get the jobs in a specific range
        public IEnumerable<Job> GetJobsByRange(int pageNum)
        {
            int correctPageNum = pageNum < 1 ? 1 : pageNum;

            return _context.Jobs.Take(correctPageNum * 10).ToList();
        }

        public void AddNewJob(Job job)
        {
            _context.Add(job);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // return 5**
            }
        }

        public Job DeleteJobById(int jobId)
        {
            var job = _context.Jobs.ToList().FirstOrDefault(job => job.Id == jobId);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                _context.SaveChanges();
            }
            return job;
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
            return _context.Jobs.ToList().Select(job => job.GetType().GetProperty(correctType)?.GetValue(job).ToString()).ToHashSet();
        }
    }
}
