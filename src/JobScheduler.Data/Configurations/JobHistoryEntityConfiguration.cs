using JobScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduler.Data.Configurations
{
    internal class JobHistoryEntityConfiguration : IEntityTypeConfiguration<JobHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<JobHistoryEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User);
            builder.HasOne(x => x.Job);

            builder.ToTable("JobStatusHistory");
        }
    }
}
