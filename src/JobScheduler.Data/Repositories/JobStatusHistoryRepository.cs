using JobScheduler.Data.Contexts;
using JobScheduler.Data.Entities;

namespace JobScheduler.Data.Repositories
{
    public class JobStatusHistoryRepository : IJobHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public JobStatusHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(JobHistoryEntity entity)
        {
            _context.JobStatusHistory.Add(entity);
        }

        public void Update(JobHistoryEntity entity)
        {
            _context.JobStatusHistory.Update(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
