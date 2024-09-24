using JobScheduler.Data.Contexts;
using JobScheduler.Data.Entities;
using JobScheduler.Shared.Enums;

namespace JobScheduler.Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobEntity entity)
        {
            await _context.Jobs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }   

        public async Task UpdateAsync(JobEntity entity)
        {
            _context.Jobs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, JobStatus scheduled)
        {
            var entry = new JobEntity { Id = id, Status = scheduled };

            _context.Jobs.Attach(entry);
            _context.Entry(entry)
                .Property(x => x.Status)
                .IsModified = true;

            await _context.SaveChangesAsync();
        }
    }
}
