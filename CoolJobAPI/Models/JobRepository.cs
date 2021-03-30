using Microsoft.EntityFrameworkCore;
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
