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

        public void Add(JobEntity entity)
        {
            _context.Jobs.Add(entity);
        }   

        public void Update(JobEntity entity)
        {
            _context.Jobs.Update(entity);
        }

        public void UpdateStatus(Guid id, JobStatus status)
        {
            var entry = new JobEntity { Id = id, Status = status };

            _context.Jobs.Attach(entry);
            _context.Entry(entry)
                .Property(x => x.Status)
                .IsModified = true;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
