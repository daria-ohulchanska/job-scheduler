using JobScheduler.Models;
using JobScheduler.Services.Scheduler;

var scheduler = new ConcurrentScheduler(capacity: 2);
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





