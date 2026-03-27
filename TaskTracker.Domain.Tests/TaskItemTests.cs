namespace TaskTracker.Domain.Tests;

public class TaskItemTests
{
    // MethodName_WhenCondition_ExpectedResult   
    // private readonly ITestOutputHelper _output;

    // ===============================================================================================================================
    // CREATE TESTS ------------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // happy path 
    // ===============================================================================================================================
    [Fact]
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

    // ===============================================================================================================================
    // defaults and state 
    // ===============================================================================================================================
    [Fact]
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

    [Fact]
    public void Create_WhenCalled_SetsStatusToActive()
    {
        // Given        
        var validTitle = "Buy milk";
        var validNotes = "From the store";        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // Then
        task.Status.Should().Be(TaskStatus.Active);
    }

    [Fact]
    public void Create_WhenCalled_SetsCreatedAtToValidCreateTime()
    {
        // Given        
        var validTitle = "Buy milk";
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // Then
        task.CreatedAt.Should().Be(validCreateTime);
    }

    [Fact]
    public void Create_WhenCalled_SetsCompletedAtToNull()
    {
        // Given        
        var validTitle = "Buy milk";
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
    
        // Then
        task.CompletedAt.Should().BeNull();
    }

    // ===============================================================================================================================
    // boundary  
    // ===============================================================================================================================
    [Fact]
    public void Create_WhenTitleIsExactly100Characters_SetsTitle()
    {
        // Given
        var titleLimit = 100;
        var titleAtLimit = new string('a', titleLimit);
        var validNotes = "From the store";        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
    
        // When
        var task = TaskItem.Create(titleAtLimit, validNotes, validCreateTime);

        // Then
        task.Title.Should().Be(titleAtLimit);
    }

    [Fact]
    public void Create_WhenTitleExceeds100Characters_ThrowsArgumentException()
    {
        // Given
        var exceedsTitleLimit = new string('a', 101);
        var validNotes = "From the store";        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(exceedsTitleLimit, validNotes, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WhenNotesIsExactly500Characters_SetsNotes()
    {
        // Given
        var notesLimit = 500;
        var validTitle = "Buy milk";
        var notesAtLimit = new string('a', notesLimit);        
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
    
        // When
        var task = TaskItem.Create(validTitle, notesAtLimit, validCreateTime);

        // Then
        task.Notes.Should().Be(notesAtLimit);
    }

    [Fact]
    public void Create_WhenNotesExceeds500Characters_ThrowsArgumentException()
    {
        // Given        
        var validTitle = "Buy milk";
        var exceedsNoteLimit = new string('a', 501);
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(validTitle, exceedsNoteLimit, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("notes");
    }

    // ===============================================================================================================================
    // validation  
    // ===============================================================================================================================
    [Fact]
    public void Create_WhenTitleIsNull_ThrowsArgumentException()
    {
        // Given   
        string? nullTitle = null;   
        var validNotes = "From the store";  
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(nullTitle, validNotes, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WhenTitleIsEmpty_ThrowsArgumentException()
    {
        // Given  
        var emptyTitle = "";   
        var validNotes = "From the store";   
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(emptyTitle, validNotes, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WhenTitleIsWhitespace_ThrowsArgumentException()
    {
        // Given   
        var whitespaceTitle = " ";   
        var validNotes = "From the store";  
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(whitespaceTitle, validNotes, validCreateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
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
    // normalization 
    // ===============================================================================================================================
    [Fact]
    public void Create_WhenTitleHasLeadingOrTrailingSpaces_TrimsTitle()
    {
        // Given        
        var titleWithWhiteSpace = "  Buy milk  ";        
        var validNotes = "From the store";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(titleWithWhiteSpace, validNotes, validCreateTime);
    
        // Then
        task.Title.Should().Be("Buy milk");        
    }

    [Fact]
    public void Create_WhenNotesHasLeadingOrTrailingSpaces_TrimsNotes()
    {
        // Given        
        var validTitle = "Buy milk";
        var notesWithWhiteSpace = "   From the store   ";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, notesWithWhiteSpace, validCreateTime);
    
        // Then
        task.Notes.Should().Be("From the store");  
    }

    [Fact]
    public void Create_WhenNotesIsWhitespace_SetsNotesToNull()
    {
        // Given        
        var validTitle = "Buy milk";
        var notesWithWhiteSpace = "  ";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, notesWithWhiteSpace, validCreateTime);
    
        // Then
        task.Notes.Should().BeNull();
    }

    [Fact]
    public void Create_WhenNotesIsEmpty_SetsNotesToNull()
    {
        // Given        
        var validTitle = "Buy milk";
        var emptyNotes = "";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, emptyNotes, validCreateTime);
    
        // Then
        task.Notes.Should().BeNull();
    }

    [Fact]
    public void Create_WhenNotesIsNull_SetsNotesToNull()
    {
        // Given        
        var validTitle = "Buy milk";
        string? nullNotes = null;
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, nullNotes, validCreateTime);
    
        // Then
        task.Notes.Should().BeNull();
    }


    // ===============================================================================================================================
    // COMPLETE TESTS ----------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // happy path     
    // ===============================================================================================================================

    [Fact]
    public void Complete_WhenTaskIsActive_SetsStatusToCompleted()
    {
        // Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

        // When        
        task.Complete(validCompleteTime);
    
        // Then       
        
        task.Status.Should().Be(TaskStatus.Completed);
    }

    [Fact]
    public void Complete_WhenTaskIsActive_SetsCompletedAt()
    {
        // Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);

        // When        
        task.Complete(validCompleteTime);
    
        // Then
        task.CompletedAt.Should().Be(validCompleteTime);
    }

    // ===============================================================================================================================
    // defaults and state
    // ===============================================================================================================================

    [Fact]
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
        task.Id.Should().Be(originalId);
        task.Title.Should().Be(originalTitle);
        task.Notes.Should().Be(originalNotes);
        task.CreatedAt.Should().Be(originalCreatedAt);
        task.Status.Should().Be(TaskStatus.Completed);
        task.CompletedAt.Should().Be(validCompleteTime);
    }
    

    [Fact]
    public void Complete_WhenTaskIsActive_DoesNotChangeId()
    {
        // Given
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var validCompleteTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        var task = TaskItem.Create(validTitle, validNotes, validCreateTime);
        var id = task.Id;

        // When
        task.Complete(validCompleteTime);

        // Then
        task.Id.Should().Be(id);
    }

    [Fact]
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
    // validation
    // ===============================================================================================================================
    [Fact]
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
    // REOPEN TESTS ------------------------------------------------------------------------------------------------------------------
    // ===============================================================================================================================

    // ===============================================================================================================================
    // happy path
    // ===============================================================================================================================







    // ===============================================================================================================================
    // defaults and state
    // ===============================================================================================================================





    // ===============================================================================================================================
    // validation tests
    // ===============================================================================================================================























    // ===============================================================================================================================
    // helpers
    // ===============================================================================================================================
    // public TaskItemTests(ITestOutputHelper output)
    // {
    //     _output = output;
    //     // _output.WriteLine($"Task title: {task.Title}");
    // }



}   // ===============================================================================================================================
