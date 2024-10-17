using JobScheduler.Services.Scheduler;
using JobScheduler.Tests.Models;
using FluentAssertions;
using Moq;
using JobScheduler.Core.Messaging;
using JobScheduler.Shared.Configurations;
using Microsoft.Extensions.Options;

namespace JobScheduler.Tests.Services.Scheduler
{
    public class ConcurrentSchedulerTests
    {
        [Fact]
        public void StubJobTest()
        {
            var settings1 = new ConcurrentSchedulerSettings { Capacity = 2 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            var scheduler = new ConcurrentScheduler(
                    mockMessageQueuePublisher.Object,
                    options1);

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