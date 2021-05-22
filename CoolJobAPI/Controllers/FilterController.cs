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
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public FilterController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // GET: api/filter/Type/Contract/1
        [Route("{filterBy}/{filterValue}/{page}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetFilteredJobs(string filterBy, string filterValue, int page)
        {
            var filtered = await _jobRepository.GetFilteredJobs(filterBy, filterValue, page);

            if (filtered == null || !filtered.Any())
            {
                return NoContent();
            }

            return Ok(filtered);
        }

        // GET: api/filter/Type
        [Route("{filterBy}")]
        public ActionResult<IEnumerable<string>> GetFilterValuesByFilterType(string filterBy)
        {
            var result = _jobRepository.GetSpecificFilterValuesByFilterType(filterBy);

            if (result == null || !result.Any())
            {
                return NoContent();
            }
            
            return Ok(result);
        }
    }
}
