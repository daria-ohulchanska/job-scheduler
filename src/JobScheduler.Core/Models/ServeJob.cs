using JobScheduler.Shared.Enums;
using JobScheduler.Shared.Extensions;

namespace JobScheduler.Core.Models
{
    public class ServeJob : IJob
    {
        private readonly int _order;
        private readonly JobType _jobType;
        private readonly TimeSpan _duration;

        public ServeJob(string userId, int order, JobType jobType, string? name = null, string? description = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? GetType().Name;
            Description = description ?? $"Serving {jobType} for order #{order}.";

            _order = order;
            _jobType = jobType;
            _duration = jobType.Duration();
        }

        public Guid Id { get; }
        public string UserId { get; }
        public string Name { get; }
        public string Description { get; }

        public async Task Run()
        {
            Console.WriteLine($"[ServeJob] Run. Processing order #{_order} ({_jobType}, will take {_duration.TotalSeconds} second(s)).");

            await Task.Delay((int)_duration.TotalMilliseconds);

            Console.WriteLine($"[ServeJob] Run. Processed order #{_order} ({_jobType}).");
        }
    }
}