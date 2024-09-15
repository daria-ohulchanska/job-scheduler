using JobScheduler.Core.Enums;
using JobScheduler.Shared.Constants;

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
