using JobScheduler.Data.Contexts;
using JobScheduler.Data.Repositories;

namespace JobScheduler.Data
{
    public interface IUnitOfWork
    {
        public IJobRepository JobRepository { get; }
        public IJobHistoryRepository JobHistoryRepository { get; }
        public ApplicationDbContext Context { get; }
        public Task SaveAsync();
    }
}
