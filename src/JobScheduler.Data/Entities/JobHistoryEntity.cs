﻿using JobScheduler.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace JobScheduler.Data.Entities;

public class JobHistoryEntity : Entity
{
    [Required]
    [ForeignKey("Job")]
    public Guid JobId { get; set; }

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    [Required]
    public Guid TransactionId { get; set; }

    [Required]
    public JobStatus Status { get; set; }

    public string? ErrorMessage { get; set; }

    public JobEntity Job { get; set; }

    public IdentityUser User { get; set; }
}