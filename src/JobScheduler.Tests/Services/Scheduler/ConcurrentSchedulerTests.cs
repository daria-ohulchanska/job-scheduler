using JobScheduler.Services.Scheduler;
using JobScheduler.Tests.Models;
using FluentAssertions;
using JobScheduler.Data.Repositories;
using Moq;
using JobScheduler.Data.Entities;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Tests.Services.Scheduler
{
    public class ConcurrentSchedulerTests
    {
        [Fact]
        public async Task StubJobTest()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.AddAsync(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository = new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));


            var scheduler = new ConcurrentScheduler(mockRepository.Object, mockHistoryRepository.Object, 2);

            var job0 = new TestJob();
            var job1 = new TestJob();
            var job2 = new TestJob();

            await scheduler.ScheduleAsync(job0);
            await scheduler.ScheduleAsync(job1);
            await scheduler.ScheduleAsync(job2);

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