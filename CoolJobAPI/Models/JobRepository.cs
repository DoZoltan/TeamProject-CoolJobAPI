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
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Jobs NOCHECK CONSTRAINT ALL");
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Users NOCHECK CONSTRAINT ALL");
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Favorites NOCHECK CONSTRAINT ALL");
            //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [Users]");
            //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [Favorites]");        
            //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [Jobs]");
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Jobs CHECK CONSTRAINT ALL");
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Users CHECK CONSTRAINT ALL");
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Favorites CHECK CONSTRAINT ALL");

            foreach (var entity in _context.Jobs)
                _context.Jobs.Remove(entity);
            foreach (var entity in _context.Users)
                _context.Users.Remove(entity);
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
                UserName = "Sanyi",
                Password = "1234",
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

        public Job GetJobById(string jobId)
        {
            var a = jobId;
            return _context.Jobs.FirstOrDefault(job => job.Id == jobId);
        }

        // Get the jobs in a specific range
        public IEnumerable<Job> GetActualJobs(int pageNum)
        {
            return _context.Jobs.Where((job, i) => i < 10 * pageNum).ToList();
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

        public IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int pageNum)
        {
            string correctValue = filterValue.Replace("%20", " ");

            // Get the job object what have the specific property (variable) with the specific value
            var filtered = _context.Jobs.ToList().Where(job => (bool)job.GetType().GetProperty(filterBy)?.GetValue(job).ToString().ToLower().Contains(correctValue)); 
            return filtered.Where((job, i) => i < 10 * pageNum);
        }

        public IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy)
        {
            // Get the unique filter values for the given filter type
            return _context.Jobs.ToList().Select(job => job.GetType().GetProperty(filterBy)?.GetValue(job).ToString());
        }
    }
}
