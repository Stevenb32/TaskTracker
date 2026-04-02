namespace TaskTracker.Domain;

public enum TaskStatus
    {
        Active,
        Completed
    }

public class TaskItem
{
    #region Properties
    // core task state
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty; // required cannot be null or whitespace
    public string? Notes { get; private set; } // optional user provided notes
    public TaskStatus Status { get; private set; } // set internally
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; } // null until the task is completed | if reopened set to null
    #endregion

    // for EF Core
    private TaskItem()
    {
    }

    // private constructor forces callers through Create so invariants stay enforced
    private TaskItem(Guid id, string title, string? notes, DateTimeOffset createdAt)
    {   
        Id = id;
        Title = title;
        Notes = notes;
        Status = TaskStatus.Active;
        CreatedAt = createdAt;
        CompletedAt = null;        
    }


    // factory method that validates and normalizes input before creating a task
    public static TaskItem Create(string title, string? notes, DateTimeOffset now)
    {    
        // title is required cannot be null empty or whitespace
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null, empty, or whitespace.", nameof(title));
        }       

        // normalize before validation so surrounding spaces do not bypass length rules
        var trimmedTitle = title.Trim();

        // title length is enforced after trimming
        if (trimmedTitle.Length > 100) 
        {
            throw new ArgumentException("Title cannot exceed 100 characters.", nameof(title));
        }

        // normalize notes before applying length and null-handling rules
        var trimmedNotes = notes?.Trim();

        // notes are optional but when provided must fit within max length
        if (trimmedNotes is not null && trimmedNotes.Length > 500) 
        {
            throw new ArgumentException("Notes cannot exceed 500 characters.", nameof(notes));
        }

        // store blank notes as null so the domain has a single representation for no notes
        var normalizedNotes = string.IsNullOrEmpty(trimmedNotes) ? null : trimmedNotes; 

        // reject default timestamps so tasks are never created with an uninitialized time
        if (now == default)
        {
            throw new ArgumentException("Now must be a valid time.", nameof(now));
        }

        return new TaskItem(
            Guid.NewGuid(),
            trimmedTitle,
            normalizedNotes,
            now);
    }


    public void Complete(DateTimeOffset now)
    {
        // reject default timestamps so completion is always recorded with a meaningful value
        if (now == default)
        {
            throw new ArgumentException(message: "Now must be a valid time.", nameof(now));
        }

        // idempotent: completing an already completed task leaves the original completion state unchanged
        if (Status == TaskStatus.Completed)
        {
            return;
        }

        // record completion and capture when it happened
        Status = TaskStatus.Completed;
        CompletedAt = now;
    }


    public void Reopen()
    {
        // idempotent: reopening a task that is not completed leaves state unchanged
        if (Status != TaskStatus.Completed)
        {
            return;
        }

        // return task to the active state and clear completed time
        Status = TaskStatus.Active;
        CompletedAt = null;
    } 

}
