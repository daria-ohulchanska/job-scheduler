using System;
using System.Collections.Generic;

namespace JobScheduler.Data.Models;

public partial class Job
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Status { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
