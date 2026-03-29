namespace TaskTracker.Domain.Tests;

public class TaskItemTests
{
    // MethodName_WhenCondition_ExpectedResult   

    // private readonly ITestOutputHelper _output;

    #region Create Tests
    // ===============================================================================================================================
    // CREATE TESTS ------------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // create title tests  
    // ===============================================================================================================================

    [Theory] // title invalid if null empty whitespace
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

    [Fact] // trimmed title length 101 is invalid
    public void Create_WhenTrimmedTitleLengthExceedsLimit_ThrowsArgumentException()
    {
        // Given
        var invalidTitleLengthAfterTrim = "   " + new string('a', 101) + "   ";
        var validNotes = "notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);

        // When
        Action act = () => TaskItem.Create(invalidTitleLengthAfterTrim, validNotes, validCreateTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Theory] // invalid title length greater than 100
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

    [Theory] // valid title length 100 char or less
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

    [Fact] // trimmed title length 100 is valid
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

    // ===============================================================================================================================
    // create notes tests  
    // ===============================================================================================================================

    [Fact] // trimmed notes lenght 501 is invalid
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

    [Theory] // invalid notes length
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

    [Theory] // notes null empty whitespace set null
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespace
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

    [Fact] // trimmed notes length 500 is valid
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

    [Theory] // valid notes length
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

    // ===============================================================================================================================
    // create time tests
    // ===============================================================================================================================

    [Fact] // default time invalid
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

    // ===============================================================================================================================
    // create happy path tests
    // ===============================================================================================================================

    [Fact] // valid inputs returns valid task
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

    [Fact] // tasks assigned different ids
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

    #endregion

    #region Complete Tests
    // ===============================================================================================================================
    // COMPLETE TESTS ----------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // complete time tests     
    // ===============================================================================================================================

    [Fact] // default time invlaid
    public void Complete_WhenTimeIsDefault_ThrowsArgumentException()
    {
        // Given
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        var defaultTime = default(DateTimeOffset);

        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // When        
        Action act = () => task.Complete(defaultTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("now");
    }

    // ===============================================================================================================================
    // complete status tests  
    // ===============================================================================================================================

    [Fact] // status equal completed do nothing
    public void Complete_WhenTaskIsAlreadyCompleted_LeavesStateUnchanged()
    {
        // Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);        

        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

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

    // ===============================================================================================================================
    // complete happy path tests
    // ===============================================================================================================================

     [Fact] // updates status to completed and completed time to now
    public void Complete_WhenTaskIsActive_OnlyUpdatesCompletionFields()
    {
        // Given
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);

        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

        var originalId = task.Id;
        var originalTitle = task.Title;
        var originalNotes = task.Notes;
        var originalCreatedAt = task.CreatedAt;

        // When
        task.Complete(validCompleteTime);

        // Then
        // not updated
        task.Id.Should().Be(originalId);
        task.Title.Should().Be(originalTitle);
        task.Notes.Should().Be(originalNotes);
        task.CreatedAt.Should().Be(originalCreatedAt);

        // updated 
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().Be(validCompleteTime);
    }

    #endregion

    #region Reopen Tests
    // ===============================================================================================================================
    // REOPEN TESTS ------------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // happy path tests
    // ===============================================================================================================================

    [Fact] // status not completed do nothing
    public void Reopen_WhenTaskStatusIsNotCompleted_LeavesStateUnchanged()
    {
        // Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);        

        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);        
        
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

    [Fact] // status not completed do nothing
    public void Reopen_WhenTaskStatusCompleted_OnlyUpdatesReopenFields()
    {
        // Given
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);

        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

        var idBeforeReopen = task.Id;
        var titleBeforeReopen = task.Title;
        var notesBeforeReopen = task.Notes;
        var createdAtBeforeReopen = task.CreatedAt;        

        task.Complete(validCompleteTime);

        // When
        task.Reopen();
    
        // Then

        //not updated
        task.Id.Should().Be(idBeforeReopen);
        task.Title.Should().Be(titleBeforeReopen);
        task.Notes.Should().Be(notesBeforeReopen);
        task.CreatedAt.Should().Be(createdAtBeforeReopen);

        // updates
        task.Status.Should().Be(TaskStatus.Active);
        task.CompletedAt.Should().BeNull();
    }

    #endregion

    #region Helper Methods
    // ===============================================================================================================================
    // helper methods
    // ===============================================================================================================================
    // public TaskItemTests(ITestOutputHelper output)
    // {
    //     _output = output;
    //     // _output.WriteLine($"Task title: {task.Title}");
    // }

    #endregion
}   // ===============================================================================================================================
