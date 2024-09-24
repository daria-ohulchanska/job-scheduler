using JobScheduler.Models;

namespace JobScheduler.Services.Scheduler
{
    public interface IScheduler
    {
        Task ScheduleAsync(IJob job);
        void Stop();
    }
}
