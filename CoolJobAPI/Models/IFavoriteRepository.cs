using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IFavoriteRepository
    {
        Task<IEnumerable<Job>> GetFavorites(int userId);
        Task<Job> GetFavoriteJob(int jobId, int userId);
        Task<bool> AddToFavorites(int jobId, int userId);
        Favorite DeleteFavoriteJob(int favId);
        int GetFavId(int jobId, int userId);
    }
}
