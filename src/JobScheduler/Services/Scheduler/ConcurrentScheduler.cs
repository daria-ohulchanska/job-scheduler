using System.Collections.Concurrent;
using JobScheduler.Models;

namespace JobScheduler.Services.Scheduler
{
    public class ConcurrentScheduler : IScheduler
    {
        private readonly object _sync = new();
        private readonly ConcurrentQueue<int> _free;
        private readonly ConcurrentQueue<IJob> _jobs;
        private readonly Task?[] _running;

        public ConcurrentScheduler(int? capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Cannot initialise scheduler: capacity must be positive");

            _running = new Task?[capacity ?? Environment.ProcessorCount];
            _jobs = new();
            _free = new();

            for (var index = 0; index < capacity; ++index)
                _free.Enqueue(index);
        }

        public void Schedule(IJob job)
        {
            if (_free.TryDequeue(out var index))
                _running[index] = Task.Run(() => Run(job, index));
            else
                _jobs.Enqueue(job);
        }

        public void Stop()
        {
            Task[] tasks;

            lock (_sync)
            {
                tasks = _running.Where(x => x is not null).ToArray()!;
            }

            Task.WaitAll(tasks);
        }

        private void Run(IJob job, int index)
        {
            while (true)
            {
                job.Run();

                if (!_jobs.TryDequeue(out job))
                {
                    _running[index] = null;
                    return;
                }
            }
        }
    }
}
