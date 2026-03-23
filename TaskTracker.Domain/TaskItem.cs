namespace TaskTracker.Domain;

public enum TaskStatus
    {
        Active,
        Completed
    }

public class TaskItem
{
    // TaskItem properties
    public Guid Id { get; } // unique Id
    public string Title { get; } 
    public string? Notes { get; } // nullable 
    public TaskStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? CompletedAt { get; private set; } // nullable

    // constructor for TaskItem
    private TaskItem(Guid id, string title, string? notes, DateTimeOffset createdAt)
    {   
        Id = id;
        Title = title;
        Notes = notes;
        Status = TaskStatus.Active;
        CreatedAt = createdAt;
        CompletedAt = null;        
    }

    // factory TaskItem Create
    public static TaskItem Create(string title, string? notes, DateTimeOffset now)
    {

        var trimmedTitle = title.Trim();
        var trimmedNotes = notes?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedTitle)) // title not null or whitespace
        {
            throw new ArgumentException("Title cannot be null, empty, or whitespace.", nameof(title));
        }       

        if (trimmedTitle.Length > 100) // title 100 char or less
        {
            throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
        }

        if (trimmedNotes is not null && trimmedNotes.Length > 500) // notes 500 char or less
        {
            throw new ArgumentException("Notes cannot exceed 500 characters.", nameof(notes));
        }

        var normalizedNotes = string.IsNullOrEmpty(trimmedNotes) ? null : trimmedNotes; // notes null or normalized 

        return new TaskItem(
            Guid.NewGuid(),
            trimmedTitle,
            normalizedNotes,
            now);
    }

    public void Complete(DateTimeOffset now)
    {        
        if (Status == TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.Completed;
        CompletedAt = now;        
    }

    public void Reopen()
    {
        if (Status != TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.Active;
        CompletedAt = null;
    }



}
