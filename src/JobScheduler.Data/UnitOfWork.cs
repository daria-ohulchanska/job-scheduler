using JobScheduler.Data.Contexts;
using JobScheduler.Data.Repositories;

namespace JobScheduler.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(ApplicationDbContext context, 
            IJobRepository jobRepository, 
            IJobHistoryRepository jobHistoryRepository)
        {
            Context = context;
            JobRepository = jobRepository;
            JobHistoryRepository = jobHistoryRepository;
        }
        
        public IJobRepository JobRepository { get; }
        public IJobHistoryRepository JobHistoryRepository { get; }
        public ApplicationDbContext Context { get; }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
