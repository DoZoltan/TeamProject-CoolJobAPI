using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolJobAPI.Models;


using Microsoft.AspNetCore.Cors;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        IJobRepository _jobRepository;

        public JobsController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        //GET: api/jobs/load
       [HttpGet("load/{AdminKey}")]
        public IActionResult GetLoad(string AdminKey)
        {
            if (AdminKey == _jobRepository.GetAdminKey()) 
            {
                _jobRepository.ClearDB();
                if (_jobRepository.GetNumberOfTheJobs() < 1)
                {                    
                    _jobRepository.LoadJson();
                }
            }
            return NoContent();
        }
        //GET: api/Jobs
        [HttpGet]
        public ActionResult<IEnumerable<Job>> GetJobs()
        {
            return Ok(_jobRepository.GetJobs().ToList());
            // NotFound() if the list is empty?
        }


        // GET: api/Jobs/5
        [HttpGet("{jobId}")]
        public ActionResult<Job> GetJob(int jobId)
        {
            var job = _jobRepository.GetJobById(jobId);

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        /*
        // PUT: api/Jobs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(string id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */
  
        //POST: api/Jobs
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Job> PostJob(Job job, int userId)
        {
            // Handle if the add procedure was failed
            _jobRepository.AddNewJob(job, userId);
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }


        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public IActionResult DeleteJob(int jobId)
        {
            var job = _jobRepository.DeleteJobById(jobId);

            if (job == null)
            {
                return NotFound();
            }

            // Should it return with the deleted job?
            return NoContent();
        }

        /*
        private bool JobExists(string id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
        */

    }
}
