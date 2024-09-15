﻿using JobScheduler.Data.Contexts;
using JobScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScheduler.Data.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<JobEntity> _set;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = _context.Set<JobEntity>();
        }

        public async Task AddAsync(JobEntity entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }   

        public async Task UpdateAsync(JobEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<JobEntity>> GetByUserIdAsync(Guid userId)
        {
            return Task.FromResult(_set.Where(x => x.UserId == userId).AsEnumerable());
        }
    }
}
