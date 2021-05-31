using CoolJobAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Interfaces
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetJobs();
        Task<int> GetNumberOfTheJobs();
        Task<Job> GetJobById(int jobId);
        Task<IEnumerable<Job>> GetJobsByRange(int pageNum);
        Task<Job> AddNewJob(Job job, string userId);
        Task<bool> DeleteJobById(int id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
        Task<bool> LoadJson(string userId);
        Task<string> GetAdminKey();
        Task<bool> ClearDB();
    }
}
