using JobScheduler.Data.Contexts;

namespace JobScheduler.Data.Repositories
{
    public interface IUnitOfWork
    {
        public IJobRepository JobRepository { get; }
        public IJobHistoryRepository JobHistoryRepository { get; }
        public ApplicationDbContext Context { get; }
        public Task SaveAsync();
    }
}
