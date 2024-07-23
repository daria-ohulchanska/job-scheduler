using JodScheduler;

var robotsCount = 2;
List<Robot> robots = new();

for (int i = 0; i <= robotsCount; i++)
    robots.Add(new Robot(i));

var scheduler = new ConcurrentScheduler(robots);
var order = -1;

void Schedule(Dish dish, double seconds)
{
    scheduler.Schedule(new ServeJob(++order, dish, TimeSpan.FromSeconds(seconds)));
    Console.WriteLine($"Scheduled order #{order}.");
}

Schedule(Dish.Crêpe, seconds: 1);
Schedule(Dish.Croissant, seconds: 1.5);
Schedule(Dish.Soufflé, seconds: 1.5);

Thread.Sleep(TimeSpan.FromSeconds(3));
Console.WriteLine("Woke up...");

Schedule(Dish.Crêpe, seconds: 2);
Schedule(Dish.Croissant, seconds: 3);
Schedule(Dish.Soufflé, seconds: 4);
Schedule(Dish.Éclair, seconds: 1);

Console.WriteLine("Stopping scheduler.");
scheduler.Stop();
Console.WriteLine("Scheduler stopped.");

Console.WriteLine("Done!");





