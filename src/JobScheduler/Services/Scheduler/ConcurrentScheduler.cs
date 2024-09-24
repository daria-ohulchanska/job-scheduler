using JobScheduler.Data.Entities;
using JobScheduler.Data.Repositories;
using JobScheduler.Models;
using JobScheduler.Shared.Enums;
using System.Diagnostics;

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

        private IJobHistoryRepository _jobHistoryRepository;
        private IJobRepository _jobRepository;

        public ConcurrentScheduler(
            IJobRepository repository, 
            IJobHistoryRepository jobHistoryRepository,
            int? capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Cannot initialise scheduler: capacity must be positive");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, $"Cannot initialise scheduler: maximum possible capacity is {MaxCapacity}");

            _jobHistoryRepository = jobHistoryRepository;
            _jobRepository = repository;

            var degreeOfParallelism = capacity ?? Environment.ProcessorCount;

            _semaphore = new SemaphoreSlim(degreeOfParallelism, degreeOfParallelism);
        }

        public async Task ScheduleAsync(IJob job)
        {
            var jobEntity = new JobEntity
            {
                UserId = job.UserId,
                Id = job.Id,
                Name = job.Name,
                Description = job.Description,
                Status = JobStatus.Pending
            };

            await _jobRepository.AddAsync(jobEntity);

            var jobHistoryEntity = new JobHistoryEntity
            {
                UserId = job.UserId,
                JobId = job.Id,
                Status = JobStatus.Pending,
            };

            await _jobHistoryRepository.AddAsync(jobHistoryEntity);

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

            var jobHistoryEntity = new JobHistoryEntity
            {
                UserId = job.UserId,
                JobId = job.Id,
            };

            try
            {
                await job.Run();
            }
            catch (Exception ex)
            {
                jobHistoryEntity.Status = JobStatus.Failed;

                await _jobHistoryRepository.AddAsync(jobHistoryEntity);
                await _jobRepository.UpdateAsync(job.Id, jobHistoryEntity.Status);

                Debug.WriteLine(ex.Message);
            }

            try
            {
                jobHistoryEntity.Status = JobStatus.Completed;

                await _jobHistoryRepository.AddAsync(jobHistoryEntity);
                await _jobRepository.UpdateAsync(job.Id, jobHistoryEntity.Status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
