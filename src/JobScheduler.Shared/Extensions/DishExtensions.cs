using JobScheduler.Shared.Constants;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Shared.Extensions
{
    public static class DishExtensions
    {
        public static TimeSpan Duration(this Dish dish)
        {
            return DishServeDuration.GetDuration(dish);
        }
    }
}
