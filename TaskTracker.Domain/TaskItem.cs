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
    // end of TaskItem properties


    // TaskItem Constructor
    private TaskItem(Guid id, string title, string? notes, DateTimeOffset createdAt)
    {   
        Id = id;
        Title = title;
        Notes = notes;
        Status = TaskStatus.Active;
        CreatedAt = createdAt;
        CompletedAt = null;        
    } // end of constructor TaskItem


    // TaskItem Create Factory
    public static TaskItem Create(string title, string? notes, DateTimeOffset now)
    {    

        // check if title is null or whitespace
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null, empty, or whitespace.", nameof(title));
        }       

        var trimmedTitle = title.Trim();

        // title 100 char or less
        if (trimmedTitle.Length > 100) 
        {
            throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
        }

        var trimmedNotes = notes?.Trim();

        // notes 500 char or less
        if (trimmedNotes is not null && trimmedNotes.Length > 500) 
        {
            throw new ArgumentException("Notes cannot exceed 500 characters.", nameof(notes));
        }

        // notes null or normalized 
        var normalizedNotes = string.IsNullOrEmpty(trimmedNotes) ? null : trimmedNotes; 

        if (now == default)
        {
            throw new ArgumentException("Now must be a valid time.", nameof(now));
        }

        return new TaskItem(
            Guid.NewGuid(),
            trimmedTitle,
            normalizedNotes,
            now);
    } // end of TaskItem Create Factory  


    public void Complete(DateTimeOffset now)
    {
        if (now == default)
        {
            throw new ArgumentException("Now must be a valid time.", nameof(now));
        }

        if (Status == TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.Completed;
        CompletedAt = now;        
    } // end of Complete


    public void Reopen()
    {
        if (Status != TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.Active;
        CompletedAt = null;
    } // end of Reopen

} // end of TaskItem
