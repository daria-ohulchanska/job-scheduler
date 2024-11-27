using JobScheduler.Shared.Enums;

namespace JobScheduler.Core.Services
{
    public interface IOrderService
    {
        Task ServeAsync(string userId, Dish dish);
    }
}
