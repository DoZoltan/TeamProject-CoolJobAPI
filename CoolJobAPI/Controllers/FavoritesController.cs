using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolJobAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CoolJobAPI.Interfaces;

namespace CoolJobAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // with this we only have access to the favorites if we logged in
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoritesController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        // Get favorites by user ID (As a user I want to see all of my favorites)
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetFavoriteJobs(string userId)
        {
            var favorites = await _favoriteRepository.GetFavorites(userId);
            
            if (favorites == null || !favorites.Any())
            {
                return NoContent();
            }

            return Ok(favorites);
        }

        // Get a specific favorite job from the user (As a user I want to get a job from my favorites and see the details of it)
        [HttpGet("{jobId}/{userId}")]
        public async Task<ActionResult<Job>> GetFavoriteJob(int jobId, string userId)
        {
            var favoriteJob = await _favoriteRepository.GetFavoriteJob(jobId, userId);

            if (favoriteJob == null)
            {
                return NoContent();
            }

            return Ok(favoriteJob);
        }

        // Add a new job to the user's favorites
        [HttpPost]
        public async Task<ActionResult<Job>> PostFavoriteJob(Job job, string userId)
        {
            var wasSuccessful = await _favoriteRepository.AddToFavorites(job.Id, userId);

            if (wasSuccessful)
            {
                return CreatedAtAction(nameof(GetFavoriteJob), new { jobId = job.Id, userId = userId }, job);
            }

            return BadRequest();
        }

        // Delete a specific job from the user's favorites
        [HttpDelete("{jobId}/{userId}")]
        public async Task<IActionResult> DeleteFavoriteJob(int jobId, string userId)
        {
            // Get the favorite id by the job and user id
            var favId = await _favoriteRepository.GetFavId(jobId, userId);

            if (await _favoriteRepository.DeleteFavoriteJob(favId) == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
 
}
