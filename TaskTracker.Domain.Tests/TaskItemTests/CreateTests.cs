namespace TaskTracker.Domain.Tests.TaskItemTests;

public class CreateTests : TaskItemTestBase
{
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

    [Theory] // title required and cannot be null empty or whitespace
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces
    public void Create_WhenTitleIsNullEmptyOrWhitespace_ThrowsArgumentException(string? title)
    {
        // Given           
        var validNotes = "From the store";  
        var validCreateTime = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(title!, validNotes, validCreateTime);
    
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

    [Theory] // notes are normalized to null instead of being stored as empty text
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

    [Fact] // valid inputs should produce a fully initialized active task
    public void Create_WhenInputsAreValid_SetsAllProperties()
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
        task.UpdatedAt.Should().BeNull();
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

}