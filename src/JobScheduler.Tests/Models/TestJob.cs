using JobScheduler.Models;

namespace JobScheduler.Tests.Models
{
    public class TestJob : IJob, IDisposable
    {
        public enum State { Pending, Running, Finished }

        private readonly Action? _before;
        private readonly Action? _after;

        public TestJob(Action? before = null, Action? after = null)
        {
            _before = before;
            _after = after;
        }

        public ManualResetEvent Started { get; } = new(initialState: false);
        public ManualResetEvent Running { get; } = new(initialState: false);

        public State CurrentState { get; private set; } = State.Pending;


        public void Run()
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
