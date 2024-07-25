using JobScheduler.Models;

namespace JobScheduler.Services
{
    public interface IScheduler
    {
        void Schedule(IJob job);
        void Stop();
    }
}
