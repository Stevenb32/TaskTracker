namespace TaskTracker.Domain.Tests.TaskItemTests;

public abstract class TaskItemTestBase
{
    protected const string ValidTitle = "Buy milk";
    protected const string ValidNotes = "From the store";

    protected static readonly DateTimeOffset ValidCreateTime = new(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
    protected static readonly DateTimeOffset ValidCompleteTime = new(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
    protected static readonly DateTimeOffset ValidUpdateTime = new(2026, 6, 7, 06, 07, 0, TimeSpan.Zero);

    protected static TaskItem CreateValidTask(string title = ValidTitle, string? notes = ValidNotes, DateTimeOffset? createdAt = null)
    {
        return TaskItem.Create(title, notes, createdAt ?? ValidCreateTime);
    }

    protected static TaskItem CreateCompletedTask( string title = ValidTitle, string? notes = ValidNotes, DateTimeOffset? createdAt = null, DateTimeOffset? completedAt = null)
    {
        var task = TaskItem.Create(title, notes, createdAt ?? ValidCreateTime);
        task.Complete(completedAt ?? ValidCompleteTime);
        return task;
    }

}