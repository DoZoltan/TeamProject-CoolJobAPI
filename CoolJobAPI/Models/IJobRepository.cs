using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IJobRepository
    {
        IEnumerable<Job> GetJobs();
        Job GetJobById(string jobId);
        IEnumerable<Job> GetActualJobs(int pageNum);
        void AddNewJob(Job job);
        Job DeleteJobById(string id);
        IEnumerable<Job> GetFilteredJobs(string filterBy, string filterValue, int num);
        IEnumerable<string> GetSpecificFilterValuesByFilterType(string filterBy);
        IEnumerable<Job> GetFavorites(int userId);
        void AddToFavorites(string jobId, int userId);
        Favorite DeleteFavoriteJob(int favId);
        int GetFavId(string jobId, int userId);
    }
}
