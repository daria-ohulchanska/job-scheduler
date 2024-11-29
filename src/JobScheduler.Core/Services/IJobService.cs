using JobScheduler.Shared.Enums;

namespace JobScheduler.Core.Services
{
    public interface IJobService
    {
        Task ScheduleAsync(string userId, JobType jobType);
    }
}
