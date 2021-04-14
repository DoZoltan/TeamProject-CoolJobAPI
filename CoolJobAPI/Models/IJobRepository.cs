using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IJobRepository
    {
        void ClearDB();
        void LoadJson();
        IEnumerable<Job> GetJobs();
        Job GetJobById(int jobId);
        IEnumerable<Job> GetJobsByRange(int pageNum);
        void AddNewJob(Job job);
        Job DeleteJobById(int id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
    }
}
