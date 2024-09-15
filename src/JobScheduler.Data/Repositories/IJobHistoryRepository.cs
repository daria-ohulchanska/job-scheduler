using JobScheduler.Data.Entities;

namespace JobScheduler.Data.Repositories
{
    public interface IJobHistoryRepository
    {
        Task AddAsync(JobHistoryEntity entity);
        Task UpdateAsync(JobHistoryEntity entity);
        Task<IEnumerable<JobHistoryEntity>> GetByUserIdAsync(Guid userId);
    }
}
