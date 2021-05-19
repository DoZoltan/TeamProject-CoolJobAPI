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
        string GetAdminKey();
        IEnumerable<Job> GetJobs();
        int GetNumberOfTheJobs();
        Job GetJobById(int jobId);
        IEnumerable<Job> GetJobsByRange(int pageNum);
        bool AddNewJob(Job job, int userId);
        bool DeleteJobById(int id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
    }
}
