namespace TaskTracker.Domain.Tests.TaskItemTests;

public abstract class TaskItemTestBase
{
    protected const string ValidTitle = "Buy milk";
    protected const string UpdatedValidTitle = "Buy eggs";

    protected const string ValidNotes = "From the store";
    protected const string UpdatedValidNotes = "From the market";

    protected const TaskPriority ValidPriority = TaskPriority.Medium;
    protected const TaskPriority UpdatedValidPriority = TaskPriority.High;

    protected static readonly DateTimeOffset DefaultTime = default;
    protected static readonly DateTimeOffset ValidCreateTime = new(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
    protected static readonly DateTimeOffset ValidUpdateTime = new(2026, 6, 7, 06, 07, 0, TimeSpan.Zero);
    protected static readonly DateTimeOffset ValidCompleteTime = new(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
    protected static readonly DateTimeOffset ValidReopenTime = new(2026, 8, 25, 11, 11, 0, TimeSpan.Zero);

    protected static TaskItem CreateValidTask(string title = ValidTitle, string? notes = ValidNotes, TaskPriority priority = ValidPriority, DateTimeOffset? createdAt = null)
    {
        return TaskItem.Create(title, notes, priority, createdAt ?? ValidCreateTime);
    }

    protected static TaskItem CreateCompletedTask(string title = ValidTitle, string? notes = ValidNotes, TaskPriority priority = ValidPriority, DateTimeOffset? createdAt = null, DateTimeOffset? completedAt = null)
    {
        var task = CreateValidTask(title, notes, priority, createdAt);

        task.Complete(completedAt ?? ValidCompleteTime);
        return task;
    }

    protected static string CreateTitleWithLength(int length)
    {
        return new string('a', length);
    }

    protected static string CreateNotesWithLength(int length)
    {
        return new string('a', length);
    }

}