using System.Collections.Concurrent;
using JobScheduler.Models;

namespace JobScheduler.Services
{
    public class ConcurrentScheduler : IScheduler
    {
        private readonly object _sync = new();
        private readonly IEnumerable<IRobot> _robots;
        private readonly ConcurrentQueue<IJob> _jobs;
        private readonly Task?[] _running;

        public ConcurrentScheduler(IEnumerable<IRobot> robots)
        {
            _robots = robots;
            _jobs = new();
            _running = new Task?[robots.Count()];
        }

        public void Schedule(IJob job)
        {
            var availableRobot = _robots.FirstOrDefault(x => !x.IsActive);
            if (availableRobot != null)
                _running[availableRobot.Id] = Task.Run(() => Run(job, availableRobot));
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

        private void Run(IJob job, IRobot robot)
        {
            robot.Start();

            while (true)
            {
                job.Run();

                if (!_jobs.TryDequeue(out job))
                {
                    _running[robot.Id] = null;
                    robot.Stop();
                    return;
                }
            }
        }
    }
}
