﻿using JobScheduler.Models;

namespace JobScheduler.Services.Scheduler
{
    public class ConcurrentScheduler : IScheduler
    {
        public const int MaxCapacity = 1024;

        private const int Running = 0;
        private const int Stopped = 1;

        private readonly object _sync = new();
        private readonly SemaphoreSlim _semaphore;

        private int _state = Running;

        public ConcurrentScheduler(int? capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Cannot initialise scheduler: capacity must be positive");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, $"Cannot initialise scheduler: maximum possible capacity is {MaxCapacity}");

            var degreeOfParallelism = capacity ?? Environment.ProcessorCount;

            _semaphore = new SemaphoreSlim(degreeOfParallelism, degreeOfParallelism);
        }

        public void Schedule(IJob job)
        {
            lock (_sync)
            {
                if (_state != Running)
                    throw new InvalidOperationException(
                        "Cannot run a job: the scheduler is not running");

                Task.Run(async () => await RunAsync(job));
            }
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

            try
            {
                job.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Job failed: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
