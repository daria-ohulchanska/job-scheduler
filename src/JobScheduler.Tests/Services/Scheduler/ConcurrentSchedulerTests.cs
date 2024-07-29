using JobScheduler.Services.Scheduler;
using JobScheduler.Tests.Models;
using FluentAssertions;
using JobScheduler.Models;

namespace JobScheduler.Tests.Services.Scheduler
{
    public class ConcurrentSchedulerTests
    {
        [Fact]
        public void StubJobTest()
        {
            var robots = new List<IRobot>(){
                new Robot(1),
                new Robot(2)
            };

            var scheduler = new ConcurrentScheduler(robots);

            var job0 = new TestJob();
            var job1 = new TestJob();
            var job2 = new TestJob();

            scheduler.Schedule(job0);
            scheduler.Schedule(job1);
            scheduler.Schedule(job2);

            job0.Started.WaitOne();
            job1.Started.WaitOne();
            job2.Started.WaitOne(TimeSpan.Zero).Should().BeFalse();

            job0.CurrentState.Should().Be(TestJob.State.Running);
            job1.CurrentState.Should().Be(TestJob.State.Running);
            job2.CurrentState.Should().Be(TestJob.State.Pending);

            job0.Finish();

            job2.Started.WaitOne();

            job0.CurrentState.Should().Be(TestJob.State.Finished);
            job1.CurrentState.Should().Be(TestJob.State.Running);
            job2.CurrentState.Should().Be(TestJob.State.Running);

            job1.Finish();
            job2.Finish();

            scheduler.Stop();

            job0.CurrentState.Should().Be(TestJob.State.Finished);
            job1.CurrentState.Should().Be(TestJob.State.Finished);
            job2.CurrentState.Should().Be(TestJob.State.Finished);
        }


    }
}