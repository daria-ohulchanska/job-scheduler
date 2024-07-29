using JobScheduler.Models;

namespace JobScheduler.Services.Scheduler
{
    public interface IScheduler
    {
        void Schedule(IJob job);
        void Stop();
    }
}
