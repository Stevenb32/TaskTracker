using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

public class TaskItemUpdateDetailsRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Notes { get; set; }
}