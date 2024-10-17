using JobScheduler.Data.Contexts;
using JobScheduler.Data.Repositories;

namespace JobScheduler.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IJobRepository _jobRepository;
        private IJobHistoryRepository _jobHistoryRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IJobRepository JobRepository => 
            _jobRepository ??= new JobRepository(Context);
        public IJobHistoryRepository JobHistoryRepository  => 
            _jobHistoryRepository ??= new JobStatusHistoryRepository(Context);
        public ApplicationDbContext Context => _context;

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }
    }
}
