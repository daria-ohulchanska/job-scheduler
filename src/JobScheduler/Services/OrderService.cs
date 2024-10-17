using JobScheduler.Core.Enums;
using JobScheduler.Data;
using JobScheduler.Data.Entities;
using JobScheduler.Models;
using JobScheduler.Services.Scheduler;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IScheduler _scheduler;
        private readonly IUnitOfWork _unitOfWork;

        private int _order = -1;

        public OrderService(IScheduler scheduler, IUnitOfWork unitOfWork)
        {
            _scheduler = scheduler;
            _unitOfWork = unitOfWork;
        }

        public async Task ServeAsync(Guid userId, Dish dish)
        {
            var job = new ServeJob(userId, ++_order, dish);

            using var transaction = _unitOfWork.Context.Database.BeginTransaction();

            var jobEntity = new JobEntity
            {
                UserId = job.UserId,
                Id = job.Id,
                Name = job.Name,
                Description = job.Description,
                Status = JobStatus.Pending
            };

            var jobHistoryEntity = new JobHistoryEntity
            {
                UserId = job.UserId,
                JobId = job.Id,
                Status = JobStatus.Pending,
                TransactionId = transaction.TransactionId
            };

            _unitOfWork.JobRepository.Add(jobEntity);
            _unitOfWork.JobHistoryRepository.Add(jobHistoryEntity);

            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            _scheduler.Schedule(job);
        }
    }
}
