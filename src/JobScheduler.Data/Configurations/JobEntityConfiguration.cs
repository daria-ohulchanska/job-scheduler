using JobScheduler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScheduler.Data.Configurations
{
    internal class JobEntityConfiguration : IEntityTypeConfiguration<JobEntity>
    {
        public void Configure(EntityTypeBuilder<JobEntity> builder)
        {
            builder.HasKey(x => x.Id);     

            builder.HasOne(x => x.User);
            builder.HasMany(x => x.History)
                .WithOne(x => x.Job);

            builder.Property(x => x.Name)
                .HasMaxLength(20);

            builder.ToTable("Jobs");
        }
    }
}
