using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolJobAPI.Models;


using Microsoft.AspNetCore.Cors;

namespace CoolJobAPI.Controllers
{
    [EnableCors("Access-Control-Allow-Origin")]
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
            if(AdminKey == "admin12345678") 
            {
                _jobRepository.ClearDB();
                if (_jobRepository.GetJobs().Count() < 1) // ef just in repository Count , ToList
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
            return _jobRepository.GetJobs().ToList(); // ef just in repository Count , ToList
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

            return job;
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
        public ActionResult<Job> PostJob(Job job)
        {
            // we need the user id who posted the new advertisement
            _jobRepository.AddNewJob(job);
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
