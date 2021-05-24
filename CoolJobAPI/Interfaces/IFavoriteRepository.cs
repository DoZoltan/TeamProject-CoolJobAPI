using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Job>> GetFavorites(string userId);
        Task<Job> GetFavoriteJob(int jobId, string userId);
        Task<bool> AddToFavorites(int jobId, string userId);
        Task<Favorite> DeleteFavoriteJob(int favId);
        Task<int> GetFavId(int jobId, string userId);
    }
}
