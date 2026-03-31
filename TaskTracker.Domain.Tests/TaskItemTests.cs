namespace TaskTracker.Domain.Tests;

public class TaskItemTests
{
    // Test naming convention: MethodName_WhenCondition_ExpectedResult   

    // private readonly ITestOutputHelper _output;
    
    // ===============================================================================================================================
    #region Create Tests
    // ===============================================================================================================================

    // ===============================================================================================================================
    #region Create Title Tests    
    // ===============================================================================================================================

    [Theory] // title required and cannot be null empty or whitespace
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    public void Create_WhenTitleIsNullEmptyOrWhitespace_ThrowsArgumentException(string? title)
    {
        // Given           
        var validNotes = "From the store";  
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(title, validNotes, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact] // guards against validating raw title length instead of trimmed length
    public void Create_WhenTrimmedTitleLengthExceedsLimit_ThrowsArgumentException()
    {
        // Given
        // surrounding spaces should not make an over-limit title pass validation
        var invalidTitleLengthAfterTrim = "   " + new string('a', 101) + "   ";
        var validNotes = "notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        Action act = () => TaskItem.Create(invalidTitleLengthAfterTrim, validNotes, validCreateTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Theory] // title must not exceed 100 characters after normalization
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public void Create_WhenTitleLengthExceedsLimit_ThrowsArgumentException(int titleLength)
    {
        // Given
        var titleWithInvalidLength = new string('a', titleLength);
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        Action act = () => TaskItem.Create(titleWithInvalidLength, validNotes, validCreateTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }    

    [Theory] // title lengths within the allowed limit should be accepted
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Create_WhenTitleLengthIsWithinLimit_SetsTitle(int titleLength)
    {
        // Given
        var validTitle = new string('a', titleLength);
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

        // Then
        task.Title.Should().Be(validTitle);
    }

    [Fact] // ensures trimming occurs before storing and validating title length
    public void Create_WhenTitleHasSpacesButTrimmedLengthIs100_SetsTrimmedTitle()
    {
        // Given
        var validTitleWithSpaces = "   " + new string('a', 100) + "   ";
        var validNotes = "notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        var task = TaskItem.Create(validTitleWithSpaces, validNotes, validCreateTime);

        // Then
        task.Title.Should().Be(new string('a', 100));
    }
    #endregion // Create Title Tests

    // ===============================================================================================================================    
    #region Create Notes Tests
    // ===============================================================================================================================

    [Fact] // guards against validating raw notes length instead of trimmed length
    public void Create_WhenTrimmedNotesLengthExceedsLimit_ThrowsArgumentException()
    {
        // Given
        var validTitle = "Buy milk";
        var invalidNotesLengthAfterTrim = "   " + new string('a', 501) + "   ";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        Action act = () => TaskItem.Create(validTitle, invalidNotesLengthAfterTrim, validCreateTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("notes");
    }

    [Theory] // notes must not exceed 500 characters after normalization
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public void Create_WhenNotesLengthExceedsLimit_ThrowsArgumentException(int notesLength)
    {
        // Given
        var validTitle = "Buy milk";
        var notesWithInvalidLength = new string('a', notesLength);        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        Action act = () => TaskItem.Create(validTitle, notesWithInvalidLength, validCreateTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("notes");
    }

    [Theory] // bank notes are normalized to null instead of being stored as empty text
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces
    public void Create_WhenNotesIsNullEmptyOrWhitespace_SetsNotesToNull(string? notes)
    {
        // Given        
        var validTitle = "Buy milk";        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, notes, validCreateTime);
    
        // Then
        task.Notes.Should().BeNull();
    }

    [Fact] // ensures trimming occurs before storing and validating notes length
    public void Create_WhenNotesHasSpacesButTrimmedLengthIs500_SetsTrimmedNotes()
    {
        // Given
        var validTitle = "Buy milk";
        var validNotesWithSpaces = "   " + new string('a', 500) + "   ";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        var task = TaskItem.Create(validTitle, validNotesWithSpaces, validCreateTime);

        // Then
        task.Notes.Should().Be(new string('a', 500));
    }

    [Theory] // notes within the allowed limit should be preserved
    [InlineData(1)]
    [InlineData(250)]
    [InlineData(500)]
    public void Create_WhenNotesLengthIsWithinLimit_SetsNotes(int notesLength)
    {
        // Given
        var validTitle = "Buy milk";
        var notesWithValidLength = new string('a', notesLength);        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        var task = TaskItem.Create(validTitle, notesWithValidLength, validCreateTime);

        // Then
        task.Notes.Should().Be(notesWithValidLength);
    }
    #endregion // Create Notes Tests

    // ===============================================================================================================================    
    #region Create Time Tests
    // ===============================================================================================================================

    [Fact] // create requires a real timestamp and rejects default value
    public void Create_WhenTimeIsDefault_ThrowsArgumentException()
    {
        // Given
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var defaultTime = default(DateTimeOffset);
    
        // When
        Action act = () => TaskItem.Create(validTitle, validNotes, defaultTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("now");
    }
    #endregion // Create Time Tests

    // ===============================================================================================================================    
    #region Create Happy Path Tests
    // ===============================================================================================================================

    [Fact] // valid inputs should produce a fully initialized active task
    public void Create_WhenInputsAreValid_ReturnsTaskItem()
    {
        // Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // Then       

        task.Id.Should().NotBeEmpty();        
        task.Title.Should().Be(validTitle);
        task.Notes.Should().Be(validNotes);
        task.Status.Should().Be(TaskStatus.Active);
        task.CreatedAt.Should().Be(validCreateTime);
        task.CompletedAt.Should().BeNull();
    }

    [Fact] // each created task should receive a unique identifier
    public void Create_WhenCalled_AssignsDifferentIds()
    {
        // Given        
        var validTitle = "Buy milk";
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task1 = TaskItem.Create(validTitle, validNotes, validCreateTime);
        var task2 = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // Then
        task1.Id.Should().NotBeEmpty();
        task2.Id.Should().NotBeEmpty();
        task1.Id.Should().NotBe(task2.Id);
    }
    #endregion // Create Happy Path Tests
    #endregion // Create Tests

    // ===============================================================================================================================
    #region Complete Tests
    // ===============================================================================================================================

    // ===============================================================================================================================    
    #region Complete Time Tests   
    // ===============================================================================================================================

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
    #endregion // Complete Time Tests

    // ===============================================================================================================================    
    #region Complete Status Tests
    // ===============================================================================================================================

    [Fact] // complete is idempotent once a task is already completed
    public void Complete_WhenTaskIsAlreadyCompleted_LeavesStateUnchanged()
    {
        // Given        
        var task = CreateValidTask();

        var firstCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        var secondCompleteTime = new DateTimeOffset(2026, 4, 21, 10, 0, 0, TimeSpan.Zero);

        task.Complete(firstCompleteTime);

        var idBeforeSecondComplete = task.Id;
        var titleBeforeSecondComplete = task.Title;
        var notesBeforeSecondComplete = task.Notes;
        var createdAtBeforeSecondComplete = task.CreatedAt;
        var statusBeforeSecondComplete = task.Status;
        var completedAtBeforeSecondComplete = task.CompletedAt;

        // When        
        task.Complete(secondCompleteTime);
    
        // Then
        task.Id.Should().Be(idBeforeSecondComplete);
        task.Title.Should().Be(titleBeforeSecondComplete);
        task.Notes.Should().Be(notesBeforeSecondComplete);
        task.Status.Should().Be(statusBeforeSecondComplete);
        task.CreatedAt.Should().Be(createdAtBeforeSecondComplete);       
        task.CompletedAt.Should().Be(completedAtBeforeSecondComplete);
    }
    #endregion // Complete Status Tests

    // ===============================================================================================================================    
    #region Complete Happy Path Tests
    // ===============================================================================================================================

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
    }
    #endregion // Complete Happy Path Tests
    #endregion // Complete Tests


    // ===============================================================================================================================    
    #region Reopen Tests
    // ===============================================================================================================================

    // ===============================================================================================================================    
    #region Reopen Happy Path Tests
    // ===============================================================================================================================

    [Fact] // reopen is a no-op unless the task is currently completed
    public void Reopen_WhenTaskStatusIsNotCompleted_LeavesStateUnchanged()
    {
        // Given
        var task = CreateValidTask();        
        
        var idBeforeReopen = task.Id;
        var titleBeforeReopen = task.Title;
        var notesBeforeReopen = task.Notes;
        var createdAtBeforeReopen = task.CreatedAt;
        var statusBeforeReopen = task.Status;
        var completedAtBeforeReopen = task.CompletedAt;

        // When        
        task.Reopen();
    
        // Then
        task.Id.Should().Be(idBeforeReopen);
        task.Title.Should().Be(titleBeforeReopen);
        task.Notes.Should().Be(notesBeforeReopen);
        task.Status.Should().Be(statusBeforeReopen);        
        task.CreatedAt.Should().Be(createdAtBeforeReopen);       
        task.CompletedAt.Should().Be(completedAtBeforeReopen);
    }

    [Fact] // reopening a completed task should only reset completion-related fields
    public void Reopen_WhenTaskStatusCompleted_OnlyUpdatesReopenFields()
    {
        // Given
        var task = CreateCompletedTask();

        var idBeforeReopen = task.Id;
        var titleBeforeReopen = task.Title;
        var notesBeforeReopen = task.Notes;
        var createdAtBeforeReopen = task.CreatedAt;

        // When
        task.Reopen();
    
        // Then

        // unchanged fields
        task.Id.Should().Be(idBeforeReopen);
        task.Title.Should().Be(titleBeforeReopen);
        task.Notes.Should().Be(notesBeforeReopen);
        task.CreatedAt.Should().Be(createdAtBeforeReopen);

        // updated fields
        task.Status.Should().Be(TaskStatus.Active);
        task.CompletedAt.Should().BeNull();
    }
    #endregion // Reopen Happy Path Tests
    #endregion // #region Reopen Tests

    #region Helper Methods
    // ===============================================================================================================================
    // shared test data and factory methods for creating TaskItem states
    // keeps test setup concise and focused on behavior under test
    // ===============================================================================================================================

    private const string ValidTitle = "Buy milk";
    private const string ValidNotes = "From the store";
    private static readonly DateTimeOffset ValidCreateTime = new(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ValidCompleteTime = new(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);

    private static TaskItem CreateValidTask(string title = ValidTitle, string? notes = ValidNotes, DateTimeOffset? createdAt = null)
    {
        return TaskItem.Create(title, notes, createdAt ?? ValidCreateTime);
    }

    private static TaskItem CreateCompletedTask(string title = ValidTitle, string? notes = ValidNotes, DateTimeOffset? createdAt = null, DateTimeOffset? completedAt = null)
    {
        var task = TaskItem.Create(title, notes, createdAt ?? ValidCreateTime);
        task.Complete(completedAt ?? ValidCompleteTime);
        return task;
    }

    // public TaskItemTests(ITestOutputHelper output)
    // {
    //     _output = output;
    //     // _output.WriteLine($"Task title: {task.Title}");
    // }

    #endregion // Helper Methods
}   // ===============================================================================================================================
