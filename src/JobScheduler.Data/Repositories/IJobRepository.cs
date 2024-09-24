using JobScheduler.Data.Entities;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Data.Repositories
{
    public interface IJobRepository
    {
        Task AddAsync(JobEntity entity);
        Task UpdateAsync(JobEntity entity);
        Task UpdateAsync(Guid id, JobStatus scheduled);
    }
}
