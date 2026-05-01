namespace TaskTracker.Domain.Tests.TaskItemTests;

public class CompleteTests : TaskItemTestBase
{
    [Fact] // complete requires a real timestamp and rejects default value
    public void Complete_WhenTimeIsDefault_ThrowsArgumentException()
    {
        // Given
        var task = CreateValidTask();        
        var defaultTime = default(DateTimeOffset);       
    
        // When        
        Action act = () => task.Complete(defaultTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("now");
    }

    [Fact] // complete is idempotent once a task is already completed
    public void Complete_WhenTaskIsAlreadyCompleted_DoesNotModifyState()
    {
        // Given        
        var task = CreateValidTask();

        var firstCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        var secondCompleteTime = new DateTimeOffset(2026, 4, 21, 10, 0, 0, TimeSpan.Zero);

        task.Complete(firstCompleteTime);

        var idBeforeSecondComplete = task.Id;
        var titleBeforeSecondComplete = task.Title;
        var notesBeforeSecondComplete = task.Notes;
        var statusBeforeSecondComplete = task.Status;
        var createdAtBeforeSecondComplete = task.CreatedAt;        
        var completedAtBeforeSecondComplete = task.CompletedAt;
        var updatedAtBeforeSecondComplete = task.UpdatedAt;

        // When        
        task.Complete(secondCompleteTime);
    
        // Then
        task.Id.Should().Be(idBeforeSecondComplete);
        task.Title.Should().Be(titleBeforeSecondComplete);
        task.Notes.Should().Be(notesBeforeSecondComplete);
        task.Status.Should().Be(statusBeforeSecondComplete);
        task.CreatedAt.Should().Be(createdAtBeforeSecondComplete);       
        task.CompletedAt.Should().Be(completedAtBeforeSecondComplete);
        task.UpdatedAt.Should().Be(updatedAtBeforeSecondComplete);
    }

     [Fact] // completing an active task should only update completion-related fields
    public void Complete_WhenTaskIsActive_OnlyUpdatesCompletionFields()
    {
        // Given        
        var task = CreateValidTask();

        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);

        var originalId = task.Id;
        var originalTitle = task.Title;
        var originalNotes = task.Notes;
        var originalCreatedAt = task.CreatedAt;

        // When
        task.Complete(validCompleteTime);

        // Then
        // unchanged fields
        task.Id.Should().Be(originalId);
        task.Title.Should().Be(originalTitle);
        task.Notes.Should().Be(originalNotes);
        task.CreatedAt.Should().Be(originalCreatedAt);

        // updated fields
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().Be(validCompleteTime);
        task.UpdatedAt.Should().Be(validCompleteTime);
    }
    
}