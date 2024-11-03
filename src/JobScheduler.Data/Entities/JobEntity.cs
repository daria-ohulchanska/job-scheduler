using JobScheduler.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobScheduler.Data.Entities
{
    public class JobEntity : Entity
    {
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required] 
        public JobStatus Status { get; set; }

        public DateTime? ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("JobId")]
        public List<JobHistoryEntity> History { get; set; }
        public UserEntity User { get; set; }
    }
}
