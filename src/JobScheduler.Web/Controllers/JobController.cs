using JobScheduler.Core.Services;
using JobScheduler.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Web.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule(string userId, JobType jobType)
        {
            await _jobService.ScheduleAsync(userId, jobType);
            return Ok();
        }
    }
}
