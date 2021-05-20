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
        Task<IEnumerable<Job>> GetJobs();
        Task<int> GetNumberOfTheJobs();
        Task<Job> GetJobById(int jobId);
        Task<IEnumerable<Job>> GetJobsByRange(int pageNum);
        Task<Job> AddNewJob(Job job, int userId);
        Task<bool> DeleteJobById(int id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
    }
}
