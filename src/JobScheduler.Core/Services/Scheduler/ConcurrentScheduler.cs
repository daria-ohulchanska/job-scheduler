using JobScheduler.Core.Messaging;
using JobScheduler.Core.Models;
using JobScheduler.Data.Entities;
using JobScheduler.Shared.Configurations;
using JobScheduler.Shared.Enums;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JobScheduler.Services.Scheduler
{
    public class ConcurrentScheduler : IScheduler
    {
        public const int MaxCapacity = 1024;

        private const int Running = 0;
        private const int Stopped = 1;

        private readonly object _sync = new();
        private readonly SemaphoreSlim _semaphore;

        private readonly IMessageQueuePublisher _messageQueuePublisher;

        private int _state = Running;

        public ConcurrentScheduler(
            IMessageQueuePublisher messageQueuePublisher,
            IOptions<ConcurrentSchedulerSettings> options)
        {
            var capacity = options.Value.Capacity;
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Cannot initialise scheduler: capacity must be positive");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, $"Cannot initialise scheduler: maximum possible capacity is {MaxCapacity}");

            _messageQueuePublisher = messageQueuePublisher;

            var degreeOfParallelism = capacity ?? Environment.ProcessorCount;

            _semaphore = new SemaphoreSlim(degreeOfParallelism, degreeOfParallelism);
        }

        public void Schedule(IJob job)
        {
            if (_state != Running)
                throw new InvalidOperationException(
                    "Cannot run a job: the scheduler is not running");

            _ = Task.Run(async() => await RunAsync(job));
        }

        public void Stop()
        {
            if (Interlocked.Exchange(ref _state, Stopped) == Stopped)
                throw new InvalidOperationException(
                    "Cannot stop the scheduler: it is already stopped");

            _semaphore.Wait();  // Block until the semaphore is available, but allows ongoing jobs to finish
        }

        private async Task RunAsync(IJob job)
        {
            await _semaphore.WaitAsync();

            var jobHistoryEntity = new JobHistoryEntity
            {
                UserId = job.UserId,
                JobId = job.Id,
            };

            try
            {
                await job.Run();

                jobHistoryEntity.Status = JobStatus.Completed;
            }
            catch (Exception ex)
            {
                jobHistoryEntity.Status = JobStatus.Failed;
                jobHistoryEntity.ErrorMessage = ex.Message;
            }
            finally
            {
                _semaphore.Release();
            }

            _messageQueuePublisher.SendMessage(JsonConvert.SerializeObject(jobHistoryEntity));
        }
    }
}
