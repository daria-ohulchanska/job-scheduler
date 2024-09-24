using JobScheduler.Core.Enums;
using JobScheduler.Shared.Extensions;

namespace JobScheduler.Models
{
    public class ServeJob : IJob
    {
        private readonly int _order;
        private readonly Dish _dish;
        private readonly TimeSpan _duration;

        public ServeJob(Guid userId, int order, Dish dish, string? name = null, string? description = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? GetType().Name;
            Description = description ?? $"Serving {dish} for order #{order}.";

            _order = order;
            _dish = dish;
            _duration = dish.Duration();
        }

        public Guid Id { get; }
        public Guid UserId { get; }
        public string Name { get; }
        public string Description { get; }

        public async Task Run()
        {
            Console.WriteLine($"[ServeJob] Run. Processing order #{_order} ({_dish}, will take {_duration.TotalSeconds} second(s)).");

            await Task.Delay((int)_duration.TotalMilliseconds);

            Console.WriteLine($"[ServeJob] Run. Processed order #{_order} ({_dish}).");
        }
    }
}