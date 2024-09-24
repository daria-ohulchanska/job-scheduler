using JobScheduler.Core.Enums;
using JobScheduler.Models;
using JobScheduler.Services.Scheduler;

namespace JobScheduler.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IScheduler _scheduler;

        private int order = -1;

        public OrderService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public async Task ServeAsync(Guid userId, Dish dish)
        {
            var job = new ServeJob(userId, ++order, dish);

            await _scheduler.ScheduleAsync(job);
        }
    }
}
