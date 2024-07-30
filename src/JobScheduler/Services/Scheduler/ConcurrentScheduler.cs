using JobScheduler.Models;

namespace JobScheduler.Services.Scheduler
{
    public class ConcurrentScheduler : IScheduler
    {
        public const int MaxCapacity = 1024;

        private const int Running = 0;
        private const int Stopped = 1;

        private readonly object _sync = new();

        private readonly Queue<int> _free;
        private readonly Queue<IJob> _jobs;
        private readonly Task?[] _running;

        private int _state = Running;

        public ConcurrentScheduler(int? capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Cannot initialise scheduler: capacity must be positive");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, $"Cannot initialise scheduler: maximum possible capacity is {MaxCapacity}");

            _running = new Task?[capacity ?? Environment.ProcessorCount];
            _jobs = new();
            _free = new();

            for (var index = 0; index < capacity; ++index)
                _free.Enqueue(index);
        }

        public void Schedule(IJob job)
        {
            lock (_sync)
            {
                if (_state != Running)
                    throw new InvalidOperationException(
                        "Cannot run a job: the scheduler is not running");

                if (_free.TryDequeue(out var index))
                    _running[index] = Task.Run(() => Run(job, index));
                else
                    _jobs.Enqueue(job);
            }
        }

        public void Stop()
        {
            if (Interlocked.Exchange(ref _state, Stopped) == Stopped)
                throw new InvalidOperationException(
                    "Cannot stop the scheduler: it is already stopped");

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

                lock (_sync)
                {
                    if (!_jobs.TryDequeue(out job))
                    {
                        _running[index] = null;
                        return;
                    }
                }
            }
        }
    }
}
