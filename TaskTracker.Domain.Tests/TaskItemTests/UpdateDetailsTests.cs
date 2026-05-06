namespace TaskTracker.Domain.Tests.TaskItemTests;

public class UpdateDetailsTests : TaskItemTestBase
{
    [Fact] // update requires a real timestamp and rejects default value
    public void UpdateDetails_WhenTimeIsDefault_ThrowsArgumentException()
    {
        // Given
        var task = CreateValidTask();

        var updatedTitle = "Updated Title";
        var updatedNotes = "Updated Notes";
        var defaultTime = default(DateTimeOffset);
    
        // When
        Action act = () => task.UpdateDetails(updatedTitle, updatedNotes, defaultTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("now");
    }

    [Theory] // title required and cannot be null empty or whitespace
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces  
    public void UpdateDetails_WhenTitleIsNullEmptyOrWhitespace_ThrowsArgumentException(string? title)
    {
        // Given           
        var task = CreateValidTask();
        
        // When
        Action act = () => task.UpdateDetails(title!, ValidNotes, ValidUpdateTime);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact] // guards against validating raw title length instead of trimmed length
    public void UpdateDetails_WhenTrimmedTitleLengthExceedsLimit_ThrowsArgumentException()
    {
        // Given
        var task = CreateValidTask();
        
        var invalidTitleLengthAfterTrim = "   " + new string('a', 101) + "   ";
        var validNotes = "notes";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);

        // When
        Action act = () => task.UpdateDetails(invalidTitleLengthAfterTrim, validNotes, validUpdatedAtTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Theory] // title must not exceed 100 characters after normalization
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public void UpdateDetails_WhenTitleLengthExceedsLimit_ThrowsArgumentException(int titleLength)
    {
        // Given

        var task = CreateValidTask();

        var titleWithInvalidLength = new string('a', titleLength);
        var validNotes = "From the store";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);

        // When
        Action act = () => task.UpdateDetails(titleWithInvalidLength, validNotes, validUpdatedAtTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact] // guards against validating raw notes length instead of trimmed length
    public void UpdateDetails_WhenTrimmedNotesLengthExceedsLimit_ThrowsArgumentException()
    {
        // Given
        var task = CreateValidTask();

        var validUpdatedTitle = "Buy 2 milk";
        var invalidNotesLengthAfterTrim = "   " + new string('a', 501) + "   ";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);


        // When
        Action act = () => task.UpdateDetails(validUpdatedTitle, invalidNotesLengthAfterTrim, validUpdatedAtTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("notes");
    }

    [Theory] // notes must not exceed 500 characters after normalization
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public void UpdateDetails_WhenNotesLengthExceedsLimit_ThrowsArgumentException(int notesLength)
    {
        // Given
        var task = CreateValidTask();

        var validUpdatedTitle = "Buy 2 milk";
        var notesWithInvalidLength = new string('a', notesLength);        
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);

        // When
        Action act = () => task.UpdateDetails(validUpdatedTitle, notesWithInvalidLength, validUpdatedAtTime);

        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("notes");
    }

    [Theory] // notes are normalized to null instead of being stored as empty text
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces
    public void UpdateDetails_WhenNotesIsNullEmptyOrWhitespace_SetsNotesToNull(string? notes)
    {
        // Given  
        var task = CreateValidTask();

        var validUpdatedTitle = "Buy 2 milk";        
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);
        
        // When
        task.UpdateDetails(validUpdatedTitle, notes, validUpdatedAtTime);
    
        // Then
        task.Notes.Should().BeNull();
    }

    [Fact]
    public void UpdateDetails_WhenTitleAndNotesAreUnchanged_DoesNotModifyState()
    {
        // Given
        var task = CreateValidTask();

        string unchnagedTitle = task.Title;
        string unchangedNotes = task.Notes!;
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);        

        var idBeforeUpdateDetails = task.Id;
        var titleBeforeUpdateDetails = task.Title;
        var notesUpdateDetails = task.Notes;
        var statusUpdateDetails = task.Status;
        var createdAtUpdateDetails = task.CreatedAt;        
        var completedAtUpdateDetails = task.CompletedAt;
        var updatedAtUpdateDetails = task.UpdatedAt;
    
        // When
        task.UpdateDetails(unchnagedTitle, unchangedNotes, validUpdatedAtTime);
    
        // Then
        task.Id.Should().Be(idBeforeUpdateDetails);
        task.Title.Should().Be(titleBeforeUpdateDetails);
        task.Notes.Should().Be(notesUpdateDetails);
        task.Status.Should().Be(statusUpdateDetails);
        task.CreatedAt.Should().Be(createdAtUpdateDetails);       
        task.CompletedAt.Should().Be(completedAtUpdateDetails);
        task.UpdatedAt.Should().Be(updatedAtUpdateDetails);
    }

    [Theory] // title lengths within the allowed limit should be accepted
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void UpdateDetails_WhenTitleLengthIsWithinLimit_SetsTitle(int titleLength)
    {
        // Given
        var task = CreateValidTask();

        var updatedValidTitle = new string('a', titleLength);
        var validNotes = "From the store";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        // When
        task.UpdateDetails(updatedValidTitle, validNotes, validUpdatedAtTime);

        // Then
        task.Title.Should().Be(updatedValidTitle);
    }

    [Fact] // ensures trimming occurs before storing and validating title length
    public void UpdateDetails_WhenTitleHasSpacesButTrimmedLengthIs100_SetsTrimmedTitle()
    {
        // Given
        var task = CreateValidTask();

        var updatedValidTitle = "   " + new string('a', 100) + "   ";
        var validNotes = "From the store";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        // When
        task.UpdateDetails(updatedValidTitle, validNotes, validUpdatedAtTime);

        // Then
        task.Title.Should().Be(new string('a', 100));
    }

    [Fact] // ensures trimming occurs before storing and validating notes length
    public void UpdateDetails_WhenNotesHasSpacesButTrimmedLengthIs500_SetsTrimmedNotes()
    {
        // Given
        var task = CreateValidTask();

        var validTitle = "Buy milk";
        var updatedValidNotesWithSpaces = "   " + new string('a', 500) + "   ";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        // When
        task.UpdateDetails(validTitle, updatedValidNotesWithSpaces, validUpdatedAtTime);

        // Then
        task.Notes.Should().Be(new string('a', 500));
    }

    [Theory] // notes within the allowed limit should be preserved
    [InlineData(1)]
    [InlineData(250)]
    [InlineData(500)]
    public void UpdateDetails_WhenNotesLengthIsWithinLimit_SetsNotes(int notesLength)
    {
        // Given
        var task = CreateValidTask();

        var validTitle = "Buy milk";
        var updatedValidNotesWithSpaces = new string('a', notesLength);        
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        // When
        task.UpdateDetails(validTitle, updatedValidNotesWithSpaces, validUpdatedAtTime);

        // Then
        task.Notes.Should().Be(updatedValidNotesWithSpaces);
    }

    [Fact] // ensures trimming occurs before storing and validating notes length
    public void UpdateDetails_WithValidTitleAndNotes_UpdatesTask()
    {
        // Given
        var task = CreateValidTask();

        var updatedValidTitle = "Buy 2 milk";
        var updatedValidNotesWithSpaces = "From the milk store";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        // When
        task.UpdateDetails(updatedValidTitle, updatedValidNotesWithSpaces, validUpdatedAtTime);

        // Then
        task.Title.Should().Be(updatedValidTitle);
        task.Notes.Should().Be(updatedValidNotesWithSpaces);
        task.UpdatedAt.Should().Be(validUpdatedAtTime);
    }

    [Fact]
    public void UpdateDetails_WithValidTitleAndNotes_DoesNotModifyNonEditableFields()
    {
        // Given
        var task = CreateValidTask();

        var updatedValidTitle = "Buy 2 milk";
        var updatedValidNotesWithSpaces = "From the milk store";
        var validUpdatedAtTime = new DateTimeOffset(2026, 6, 07, 06, 07, 0, TimeSpan.Zero);  

        var idBeforeUpdateDetails = task.Id;
        var statusUpdateDetails = task.Status;
        var createdAtUpdateDetails = task.CreatedAt;        
        var completedAtUpdateDetails = task.CompletedAt;
        

        // When
        task.UpdateDetails(updatedValidTitle, updatedValidNotesWithSpaces, validUpdatedAtTime);

        // Then
        task.Id.Should().Be(idBeforeUpdateDetails);        
        task.Status.Should().Be(statusUpdateDetails);
        task.CreatedAt.Should().Be(createdAtUpdateDetails);       
        task.CompletedAt.Should().Be(completedAtUpdateDetails);
        
    }

}