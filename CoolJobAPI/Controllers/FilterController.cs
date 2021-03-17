using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoolJobAPI.Models;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Cors;

namespace CoolJobAPI.Controllers
{
    [EnableCors("Access-Control-Allow-Origin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public FilterController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // GET: api/filter/Type/Contract/1
        [Route("{filterBy}/{filterValue}/{page}")]
        [HttpGet("{filterBy}/{filterValue}/{page}")]
        public ActionResult<IEnumerable<Job>> GetFilteredJobs(string filterBy, string filterValue, int page)
        {
            return _jobRepository.GetFilteredJobs(filterBy, filterValue.ToLower(), page).ToList();
        }

        // GET: api/filter/Type
        [Route("{filterBy}")]
        [HttpGet("{filterBy}")]
        public ActionResult<IEnumerable<string>> GetFilterValuesByFilterType(string filterBy)
        {
            var result = _jobRepository.GetSpecificFilterValuesByFilterType(filterBy).ToHashSet();
            result.Remove("");
            return result;
        }
    }
}
