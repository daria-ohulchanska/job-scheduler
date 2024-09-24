using JobScheduler.Core.Enums;

namespace JobScheduler.Shared.Constants
{
    public static class DishServeDuration
    {
        public const int Fast = 1;
        public const int Medium = 2;
        public const int Slow = 3;

        public static TimeSpan GetDuration(Dish dish)
        {
            var minutes =  dish switch
            {
                Dish.Soufflé => Fast,
                Dish.Éclair => Medium,
                Dish.Croissant => Medium,
                Dish.Crêpe => Slow,
                _ => 0,
            };

            return TimeSpan.FromMinutes(minutes);
        }
    }
}
