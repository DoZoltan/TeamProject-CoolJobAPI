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
        public ActionResult<IEnumerable<Job>> GetFilteredJobs(string filterBy, string filterValue, int page)
        {
            var filtered = _jobRepository.GetFilteredJobs(filterBy, filterValue, page);

            if (filtered == null || filtered.ToList().Count == 0)
            {
                return NoContent();
            }

            return Ok(filtered);
        }

        // GET: api/filter/Type
        [Route("{filterBy}")]
        public ActionResult<IEnumerable<string>> GetFilterValuesByFilterType(string filterBy)
        {
            var result = _jobRepository.GetSpecificFilterValuesByFilterType(filterBy).ToHashSet();
            result.Remove("");
            return Ok(result);
            // NotFound() if the list is empty?
        }
    }
}
