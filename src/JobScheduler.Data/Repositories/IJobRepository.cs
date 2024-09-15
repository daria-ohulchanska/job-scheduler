using JobScheduler.Data.Entities;

namespace JobScheduler.Data.Repositories
{
    public interface IJobRepository
    {
        Task AddAsync(JobEntity entity);
        Task UpdateAsync(JobEntity entity);
        Task<IEnumerable<JobEntity>> GetByUserIdAsync(Guid userId);
    }
}
