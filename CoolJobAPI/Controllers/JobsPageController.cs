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

namespace CoolJobAPI.Controllers
{
    [EnableCors("Access-Control-Allow-Origin")]
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
        public ActionResult<IEnumerable<Job>> GetJobsByPage(int page)
        {
            return _jobRepository.GetActualJobs(page).ToList();
        }
    }
}
