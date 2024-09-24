using JobScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScheduler.Data.Contexts
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : DbContext(options)
    {
        public DbSet<JobEntity> Jobs { get; set; }
        public DbSet<JobHistoryEntity> JobStatusHistory { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
