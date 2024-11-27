using JobScheduler.Core.Models;

namespace JobScheduler.Tests.Models
{
    public class TestJob : IJob, IDisposable
    {
        public enum State { Pending, Running, Finished }

        private readonly Action? _before;
        private readonly Action? _after;

        public TestJob(Action? before = null, Action? after = null)
        {
            UserId = Guid.NewGuid().ToString();
            Id = Guid.NewGuid();
            Name = GetType().Name;
            Description = $"Test job {Id} for user {UserId}";

            _before = before;
            _after = after;
        }

        public ManualResetEvent Started { get; } = new(initialState: false);
        public ManualResetEvent Running { get; } = new(initialState: false);

        public State CurrentState { get; private set; } = State.Pending;

        public Guid Id { get; }
        public string UserId { get; }
        public string Name { get; }
        public string Description { get;  } 

        public async Task Run()
        {
            CurrentState = State.Running;
            Started.Set();

            _before?.Invoke();
            Running.WaitOne();
            _after?.Invoke();

            CurrentState = State.Finished;
        }

        public void Finish()
        {
            if (CurrentState is not State.Running)
                throw new InvalidOperationException(
                    $"Cannot stop a test job: current state is `{CurrentState}`");

            Running.Set();
        }

        public void Dispose() =>
            Running.Dispose();
    }
}
