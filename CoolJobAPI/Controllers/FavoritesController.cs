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
    [EnableCors("Access-Control-Allow-Origin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public FavoritesController (IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // Get favorites by user ID (As a user I want to see all of my favorites)
        [HttpGet]
        public ActionResult<IEnumerable<Job>> GetFavoriteJobs(int userId = 0)
        {
            return _jobRepository.GetFavorites(userId).ToList();
        }

        // Get a specific favorite job from the user (As a user I want to get a job from my favorites and see the details of it)
        [HttpGet("{id}")]
        public ActionResult<Job> GetFavoriteJob(string jobId, int userId = 0)
        {
            var job = _jobRepository.GetFavorites(userId).FirstOrDefault(job => job.Id == jobId);

            if (job == null)
            {
                return NotFound();
            }

            return job;
        }

        // Add a new job to the user's favorites
        [HttpPost]
        public ActionResult<Job> PostFavoriteJob(Job job)
        {
            int userId = 0;
            _jobRepository.AddToFavorites(job.Id, userId);
            return CreatedAtAction(nameof(GetFavoriteJob), new { job.Id, userId }, job);
        }

        // Delete a specific job from the user's favorites
        [HttpDelete("{jobId}/{userId}")]
        public IActionResult DeleteFavoriteJob(string jobId, int userId = 0)
        {
            // Get the favorite id by the job and user id
            var favId = _jobRepository.GetFavId(jobId, userId);

            if (_jobRepository.DeleteFavoriteJob(favId) == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
 
}
