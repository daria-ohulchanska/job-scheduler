using JobScheduler.Data.Contexts;
using JobScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScheduler.Data.Repositories
{
    public class JobStatusHistoryRepository : IJobHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<JobHistoryEntity> _set;

        public JobStatusHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = _context.Set<JobHistoryEntity>();
        }

        public async Task AddAsync(JobHistoryEntity entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(JobHistoryEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<JobHistoryEntity>> GetByUserIdAsync(Guid userId)
        {
            return Task.FromResult(_set.Where(x => x.UserId == userId).AsEnumerable());
        }

        public Task<IEnumerable<JobHistoryEntity>> GetByJobIdAsync(Guid jobId)
        {
            return Task.FromResult(_set.Where(x => x.JobId == jobId).AsEnumerable());
        }
    }
}
