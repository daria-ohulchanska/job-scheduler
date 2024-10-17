using JobScheduler.Data.Entities;

namespace JobScheduler.Data.Repositories
{
    public interface IJobHistoryRepository
    {
        void Add(JobHistoryEntity entity);
        void Update(JobHistoryEntity entity);
        Task SaveAsync();
    }
}
