using JobScheduler.Data.Models;
using JobScheduler.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobScheduler.Data.Entities
{
    public class JobHistoryEntity : Entity
    {
        [Required]
        [ForeignKey("Job")]
        public Guid JobId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public JobStatus Status { get; set; }

        public string? ErrorMessage { get; set; }

        public JobEntity Job { get; set; }

        public User User { get; set; }
    }
}
