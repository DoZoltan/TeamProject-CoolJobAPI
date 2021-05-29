using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolJobAPI.Models;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using CoolJobAPI.Interfaces;

namespace CoolJobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsPageController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public JobsPageController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // GET: api/JobsPage/5
        [HttpGet("{page}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByPage(int page)
        {
            var jobs = await _jobRepository.GetJobsByRange(page);

            if (jobs == null || !jobs.Any())
            {
                return NoContent();
            }

            return Ok(jobs);
        }
    }
}
