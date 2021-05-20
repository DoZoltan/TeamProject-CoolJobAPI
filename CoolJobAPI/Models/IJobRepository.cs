using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IJobRepository
    {
        Task<bool> ClearDB();
        Task<bool> LoadJson();
        Task<string> GetAdminKey();
        IEnumerable<Job> GetJobs();
        int GetNumberOfTheJobs();
        Job GetJobById(int jobId);
        Task<IEnumerable<Job>> GetJobsByRange(int pageNum);
        Job AddNewJob(Job job, int userId);
        bool DeleteJobById(int id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
    }
}
