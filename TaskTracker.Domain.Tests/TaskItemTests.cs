namespace TaskTracker.Domain.Tests;

public class TaskItemTests
{
    // MethodName_WhenCondition_ExpectedResult   
    private readonly ITestOutputHelper _output;

    // Create Tests ==================================================================================================================
    // happy path tests --------------------------------------------------------------------------------------------------------------
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

    // defaults and state tests -----------------------------------------------------------------------------------------------------
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

    // Boundary tests -----------------------------------------------------------------------------------------------------
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
    public void Create_WhenNotesExceed500Characters_ThrowsArgumentException()
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


    // validation tests -----------------------------------------------------------------------------------------------------
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

    // normalization tests -----------------------------------------------------------------------------------------------------
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



    // Close Tests ===================================================================================================================
    // happy path tests --------------------------------------------------------------------------------------------------------------    
    [Fact]
    public void TestName()
    {
        /// Given        
        var validTitle = "Valid Title";
        var validNotes = "Valid Notes";
        var createdTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var completedTime = new DateTimeOffset(2026, 4, 20, 16, 20, 0, TimeSpan.Zero);
        
        // When
        var task = TaskItem.Create(validTitle, validNotes, createdTime);
        task.Complete(completedTime);       
    
        // Then
    
    }

    // defaults and state tests -----------------------------------------------------------------------------------------------------




    // Reopen Tests ==================================================================================================================



    // helpers
    public TaskItemTests(ITestOutputHelper output)
    {
        _output = output;
        // _output.WriteLine($"Task title: {task.Title}");
    }


} // end TaskItemTests
