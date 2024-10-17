namespace JobScheduler.Data.Models;

public partial class JobStatusHistory
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public Guid UserId { get; set; }

    public Guid TransactionId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? ErrorMessage { get; set; }
}
