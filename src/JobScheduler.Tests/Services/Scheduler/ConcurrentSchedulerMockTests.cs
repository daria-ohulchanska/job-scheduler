﻿using FluentAssertions;
using JobScheduler.Models;
using JobScheduler.Services.Scheduler;
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
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConcurrentScheduler(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ConcurrentScheduler(-1));
        }

        [Fact]
        public void Schedule_ExecutesJobImmediately_IfCapacityIsAvailable()
        {
            var scheduler = new ConcurrentScheduler(1);
            var mockJob = CreateMockJob();

            scheduler.Schedule(mockJob.Object);

            Thread.Sleep(200); // Wait for the job to complete
            mockJob.Verify(job => job.Run(), Times.Once);
        }

        [Fact]
        public void Schedule_QueuesJob_IfCapacityIsNotAvailable()
        {
            var scheduler = new ConcurrentScheduler(1);
            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();

            scheduler.Schedule(mockJob1.Object); // This should run immediately
            scheduler.Schedule(mockJob2.Object); // This should be queued

            Thread.Sleep(50); // Wait for the first job to complete
            mockJob1.Verify(job => job.Run(), Times.Once);
            mockJob2.Verify(job => job.Run(), Times.Never); // Second job should not run yet

            Thread.Sleep(200); // Wait for the second job to run
            mockJob2.Verify(job => job.Run(), Times.Once);
        }

        [Fact]
        public void Stop_WaitsForAllRunningJobsToComplete()
        {
            var scheduler = new ConcurrentScheduler(2);
            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();
            var mockJob3 = CreateMockJob();

            scheduler.Schedule(mockJob1.Object);
            scheduler.Schedule(mockJob2.Object);
            scheduler.Schedule(mockJob3.Object);

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
            var scheduler = new ConcurrentScheduler(1);
            var mockJob1 = CreateMockJob();
            var mockJob2 = CreateMockJob();
            var mockJob3 = CreateMockJob();

            scheduler.Schedule(mockJob1.Object);
            scheduler.Schedule(mockJob2.Object);
            scheduler.Schedule(mockJob3.Object);

            Thread.Sleep(600); // Wait for all jobs to complete

            mockJob1.Verify(job => job.Run(), Times.Once);
            mockJob2.Verify(job => job.Run(), Times.Once);
            mockJob3.Verify(job => job.Run(), Times.Once);
        }
    }
}