using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolJobAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoritesController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        // Get favorites by user ID (As a user I want to see all of my favorites)
        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<Job>> GetFavoriteJobs(int userId)
        {
            return _favoriteRepository.GetFavorites(userId).ToList();
        }

        // Get a specific favorite job from the user (As a user I want to get a job from my favorites and see the details of it)
        [HttpGet("{jobId}/{userId}")]
        public ActionResult<Job> GetFavoriteJob(int jobId, int userId)
        {
            return _favoriteRepository.GetFavorites(userId).FirstOrDefault(job => job.Id == jobId);
        }

        // Add a new job to the user's favorites
        [HttpPost]
        public ActionResult<Job> PostFavoriteJob(Job job, int userId)
        {
            // Handle if the add procedure was failed
            var wasSuccessful = _favoriteRepository.AddToFavorites(job.Id, userId);
            return GetFavoriteJob(job.Id, userId);
        }

        // Delete a specific job from the user's favorites
        [HttpDelete("{jobId}/{userId}")]
        public IActionResult DeleteFavoriteJob(int jobId, int userId)
        {
            // Get the favorite id by the job and user id
            var favId = _favoriteRepository.GetFavId(jobId, userId);

            if (_favoriteRepository.DeleteFavoriteJob(favId) == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
 
}
