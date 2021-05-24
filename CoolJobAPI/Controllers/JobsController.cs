﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolJobAPI.Models;


using Microsoft.AspNetCore.Cors;
using System.Threading.Tasks;

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

        /*
        //GET: api/jobs/load
        [HttpGet("load/{AdminKey}")]
        public async Task<IActionResult> GetLoad(string AdminKey)
        {
            if (AdminKey == await _jobRepository.GetAdminKey()) 
            {
                if (await _jobRepository.ClearDB() && await _jobRepository.GetNumberOfTheJobs() < 1)
                {                    
                   await _jobRepository.LoadJson();
                }
            }
            return NoContent();
        }
        */

        //GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            var jobs = await _jobRepository.GetJobs();

            if (jobs == null || !jobs.Any())
            {
                return NoContent();
            }

            return Ok(jobs);
        }


        // GET: api/Jobs/5
        [HttpGet("{jobId}")]
        public async Task<ActionResult<Job>> GetJob(int jobId)
        {
            var job = await _jobRepository.GetJobById(jobId);

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
        public async Task<ActionResult<Job>> PostJob(Job job, int userId)
        {
            var addedJob = await _jobRepository.AddNewJob(job, userId);

            if (addedJob != null)
            {
                return Ok(addedJob); //CreatedAtAction(nameof(GetJob), new { id = addedJob.Id }, addedJob);
            }

            return BadRequest();
        }

        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> DeleteJob(int jobId)
        {
            var job = await _jobRepository.GetJobById(jobId);

            if (job == null)
            {
                return NotFound();
            }
            else
            {
                var success = await _jobRepository.DeleteJobById(jobId);

                if (success)
                {
                    return CreatedAtAction(nameof(GetJob), new { id = jobId }, job);
                }
            }

            return BadRequest();
        }

        /*
        private bool JobExists(string id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
        */

    }
}
