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

        public async Task AddAsync(JobHistoryEntity entity)
        {
            await _context.JobStatusHistory.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobHistoryEntity entity)
        {
            _context.JobStatusHistory.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
