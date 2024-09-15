using JobScheduler.Core.Enums;
using JobScheduler.Shared.Extensions;

namespace JobScheduler.Models
{
    public class ServeJob : IJob
    {
        private readonly int _order;
        private readonly Dish _dish;
        private readonly TimeSpan _duration;

        public ServeJob(int order, Dish dish)
        {
            Name = GetType().Name;
            Description = $"Serving {dish} for order #{order}.";

            _order = order;
            _dish = dish;
            _duration = dish.Duration();
        }

        public string Name { get; }
        public string Description { get; }

        public void Run()
        {
            Console.WriteLine($"Processing order #{_order} ({_dish}, will take {_duration.TotalSeconds} second(s)).");
            Thread.Sleep(_duration);
            Console.WriteLine($"Processed order #{_order} ({_dish}).");
        }
    }
}
