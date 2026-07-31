using System.ComponentModel.DataAnnotations;
using TaskTracker.Domain;

namespace TaskTracker.Api.Dtos;
public class TaskItemCreateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}