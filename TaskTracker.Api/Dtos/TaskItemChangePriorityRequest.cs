using System.ComponentModel.DataAnnotations;
using TaskTracker.Domain;

namespace TaskTracker.Api.Dtos;

public class TaskItemChangePriorityRequest
{
    [Required]
    public TaskPriority? Priority { get; set; }
}