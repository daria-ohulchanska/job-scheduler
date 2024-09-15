using JobScheduler.Core.Enums;
using JobScheduler.Models;
using JobScheduler.Services.Scheduler;

namespace JobScheduler.Core.Services
{
    public class OrderService
    {
        private readonly IScheduler _scheduler;
        private int order = -1;

        public OrderService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Serve(Dish dish)
        {
            var job = new ServeJob(++order, dish);
            _scheduler.Schedule(job);
        }
    }
}
