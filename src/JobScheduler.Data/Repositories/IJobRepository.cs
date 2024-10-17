using JobScheduler.Data.Entities;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Data.Repositories
{
    public interface IJobRepository
    {
        void Add(JobEntity entity);
        void Update(JobEntity entity);
        void UpdateStatus(Guid id, JobStatus status);
        Task SaveAsync();
    }
}
