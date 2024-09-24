using JobScheduler.Core.Enums;

namespace JobScheduler.Core.Services
{
    public interface IOrderService
    {
        Task ServeAsync(Guid userId, Dish dish);
    }
}
