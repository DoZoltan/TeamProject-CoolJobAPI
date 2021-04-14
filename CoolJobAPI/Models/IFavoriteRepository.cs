using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IFavoriteRepository
    {
        IEnumerable<Job> GetFavorites(int userId);
        bool AddToFavorites(int jobId, int userId);
        Favorite DeleteFavoriteJob(int favId);
        int GetFavId(int jobId, int userId);
    }
}
