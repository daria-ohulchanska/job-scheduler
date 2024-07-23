namespace JodScheduler
{
    public class ServeJob : IJob
    {
        private readonly int _order;
        private readonly Dish _dish;
        private readonly TimeSpan _duration;

        public ServeJob(int order, Dish dish, TimeSpan duration)
        {
            _order = order;
            _dish = dish;
            _duration = duration;
        }

        public void Run()
        {
            Console.WriteLine($"Processing order #{_order} ({_dish}, will take {_duration.TotalSeconds} second(s)).");
            Thread.Sleep(_duration);
            Console.WriteLine($"Processed order #{_order} ({_dish}).");
        }
    }
}
