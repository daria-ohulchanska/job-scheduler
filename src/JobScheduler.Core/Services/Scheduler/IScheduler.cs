using JobScheduler.Core.Models;

namespace JobScheduler.Services.Scheduler
{
    public interface IScheduler
    {
        void Schedule(IJob job);
        void Stop();
    }
}
