using FluentAssertions;
using JobScheduler.Core.Messaging;
using JobScheduler.Data.Entities;
using JobScheduler.Data.Repositories;
using JobScheduler.Models;
using JobScheduler.Services.Scheduler;
using JobScheduler.Shared.Configurations;
using JobScheduler.Shared.Enums;
using Microsoft.Extensions.Options;
using Moq;

namespace JobScheduler.Tests.Services.Scheduler
{
    public class ConcurrentSchedulerMockTests
    {
        private Mock<IJob> CreateMockJob()
        {
            var mockJob = new Mock<IJob>();
            mockJob.Setup(job => job.Run()).Callback(() => Thread.Sleep(100)); // Simulate some work
            return mockJob;
        }

        [Fact]
        public void Constructor_Throws_IfCapacityIsNotPositive()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.Add(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.Update(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository= new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));

            var settings1 = new ConcurrentSchedulerSettings { Capacity = 1 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var settings2 = new ConcurrentSchedulerSettings { Capacity = 2 };
            IOptions<ConcurrentSchedulerSettings> options2 = Options.Create(settings2);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new ConcurrentScheduler(
                    mockRepository.Object, 
                    mockHistoryRepository.Object, 
                    mockMessageQueuePublisher.Object,  
                    options1));

            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new ConcurrentScheduler(
                    mockRepository.Object, 
                    mockHistoryRepository.Object,
                    mockMessageQueuePublisher.Object,
                    options2));
        }

        [Fact]
        public void Schedule_ExecutesJobImmediately_IfCapacityIsAvailable()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.Add(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.Update(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository = new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));

            var settings1 = new ConcurrentSchedulerSettings { Capacity = 1 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            var scheduler = new ConcurrentScheduler(
                    mockRepository.Object,
                    mockHistoryRepository.Object,
                    mockMessageQueuePublisher.Object,
                    options1);

            var mockJob = CreateMockJob();

            scheduler.ScheduleAsync(mockJob.Object);

            Thread.Sleep(200); // Wait for the job to complete
            mockJob.Verify(job => job.Run(), Times.Once);
        }

        [Fact]
        public void Schedule_QueuesJob_IfCapacityIsNotAvailable()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.Add(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.Update(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository = new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));

            var settings1 = new ConcurrentSchedulerSettings { Capacity = 1 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            var scheduler = new ConcurrentScheduler(
                    mockRepository.Object,
                    mockHistoryRepository.Object,
                    mockMessageQueuePublisher.Object,
                    options1);

            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();

            scheduler.ScheduleAsync(mockJob1.Object); // This should run immediately
            scheduler.ScheduleAsync(mockJob2.Object); // This should be queued

            Thread.Sleep(50); // Wait for the first job to complete
            mockJob1.Verify(job => job.Run(), Times.Once);
            mockJob2.Verify(job => job.Run(), Times.Never); // Second job should not run yet

            Thread.Sleep(200); // Wait for the second job to run
            mockJob2.Verify(job => job.Run(), Times.Once);
        }

        [Fact]
        public void Stop_WaitsForAllRunningJobsToComplete()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.Add(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.Update(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository = new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));

            var settings1 = new ConcurrentSchedulerSettings { Capacity = 2 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            var scheduler = new ConcurrentScheduler(
                    mockRepository.Object,
                    mockHistoryRepository.Object,
                    mockMessageQueuePublisher.Object,
                    options1);

            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();
            var mockJob3 = CreateMockJob();

            scheduler.ScheduleAsync(mockJob1.Object);
            scheduler.ScheduleAsync(mockJob2.Object);
            scheduler.ScheduleAsync(mockJob3.Object);

            Thread.Sleep(150); // Ensure jobs have started

            Task stopTask = Task.Run(scheduler.Stop);
            stopTask.IsCompleted.Should().BeFalse();

            Thread.Sleep(200); // Wait for jobs to complete

            stopTask.IsCompleted.Should().BeTrue();

            mockJob1.Verify(job => job.Run(), Times.Once);
            mockJob2.Verify(job => job.Run(), Times.Once);
            mockJob3.Verify(job => job.Run(), Times.Once);
        }

        [Fact]
        public void Run_ExecutesJobsUntilQueueIsEmpty()
        {
            var mockRepository = new Mock<IJobRepository>();
            mockRepository.Setup(x => x.Add(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.Update(It.IsAny<JobEntity>()));
            mockRepository.Setup(x => x.UpdateStatus(It.IsAny<Guid>(), It.IsAny<JobStatus>()));

            var mockHistoryRepository = new Mock<IJobHistoryRepository>();
            mockHistoryRepository.Setup(x => x.AddAsync(It.IsAny<JobHistoryEntity>()));
            mockHistoryRepository.Setup(x => x.UpdateAsync(It.IsAny<JobHistoryEntity>()));

            var settings1 = new ConcurrentSchedulerSettings { Capacity = 1 };
            IOptions<ConcurrentSchedulerSettings> options1 = Options.Create(settings1);

            var mockMessageQueuePublisher = new Mock<IMessageQueuePublisher>();
            mockMessageQueuePublisher.Setup(x => x.SendMessage(It.IsAny<string>()));

            var scheduler = new ConcurrentScheduler(
                    mockRepository.Object,
                    mockHistoryRepository.Object,
                    mockMessageQueuePublisher.Object,
                    options1);

            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();
            var mockJob3 = CreateMockJob();

            scheduler.ScheduleAsync(mockJob1.Object);
            scheduler.ScheduleAsync(mockJob2.Object);
            scheduler.ScheduleAsync(mockJob3.Object);

            Thread.Sleep(600); // Wait for all jobs to complete

            mockJob1.Verify(job => job.Run(), Times.Once);
            mockJob2.Verify(job => job.Run(), Times.Once);
            mockJob3.Verify(job => job.Run(), Times.Once);
        }
    }
}
