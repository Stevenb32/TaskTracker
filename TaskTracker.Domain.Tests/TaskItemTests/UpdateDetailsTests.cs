using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Domain.Tests.TaskItemTests;

public class UpdateDetailsTests : TaskItemTestBase
{
    [Fact] // update requires a real timestamp and rejects default value
    public void UpdateDetails_WhenTimeIsDefault_ThrowsDomainValidationException()
    {
        // Given
        var task = CreateValidTask();

        // When
        Action act = () => task.UpdateDetails(UpdatedValidTitle, UpdatedValidNotes, DefaultTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("now").WithMessage("*Now must be a valid time.*");
    }

    [Theory] // title required and cannot be null empty or whitespace
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces  
    public void UpdateDetails_WhenTitleIsNullEmptyOrWhitespace_ThrowsDomainValidationException(string? title)
    {
        // Given           
        var task = CreateValidTask();

        // When
        Action act = () => task.UpdateDetails(title!, ValidNotes, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot be null, empty, or whitespace.*");
    }

    [Fact] // guards against validating raw title length instead of trimmed length
    public void UpdateDetails_WhenTrimmedTitleLengthExceedsLimit_ThrowsDomainValidationException()
    {
        // Given
        var task = CreateValidTask();

        var invalidTitleLengthAfterTrim = "   " + new string('a', 101) + "   ";

        // When
        Action act = () => task.UpdateDetails(invalidTitleLengthAfterTrim, ValidNotes, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot exceed 100 characters.*");
    }

    [Theory] // title must not exceed 100 characters after normalization
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public void UpdateDetails_WhenTitleLengthExceedsLimit_ThrowsDomainValidationException(int titleLength)
    {
        // Given

        var task = CreateValidTask();

        var titleWithInvalidLength = new string('a', titleLength);

        // When
        Action act = () => task.UpdateDetails(titleWithInvalidLength, ValidNotes, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot exceed 100 characters.*");
    }

    [Fact] // guards against validating raw notes length instead of trimmed length
    public void UpdateDetails_WhenTrimmedNotesLengthExceedsLimit_ThrowsDomainValidationException()
    {
        // Given
        var task = CreateValidTask();

        var invalidNotesLengthAfterTrim = "   " + new string('a', 501) + "   ";

        // When
        Action act = () => task.UpdateDetails(UpdatedValidTitle, invalidNotesLengthAfterTrim, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("notes").WithMessage("*Notes cannot exceed 500 characters.*");
    }

    [Theory] // notes must not exceed 500 characters after normalization
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public void UpdateDetails_WhenNotesLengthExceedsLimit_ThrowsDomainValidationException(int notesLength)
    {
        // Given
        var task = CreateValidTask();

        var notesWithInvalidLength = new string('a', notesLength);

        // When
        Action act = () => task.UpdateDetails(UpdatedValidTitle, notesWithInvalidLength, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("notes").WithMessage("*Notes cannot exceed 500 characters.*");
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

        // When
        task.UpdateDetails(UpdatedValidTitle, notes, ValidUpdateTime);

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

        var idBeforeUpdateDetails = task.Id;
        var titleBeforeUpdateDetails = task.Title;
        var notesUpdateDetails = task.Notes;
        var statusUpdateDetails = task.Status;
        var priorityUpdateDetails = task.Priority;
        var createdAtUpdateDetails = task.CreatedAt;
        var completedAtUpdateDetails = task.CompletedAt;
        var updatedAtUpdateDetails = task.UpdatedAt;

        // When
        task.UpdateDetails(unchnagedTitle, unchangedNotes, ValidUpdateTime);

        // Then
        task.Id.Should().Be(idBeforeUpdateDetails);
        task.Title.Should().Be(titleBeforeUpdateDetails);
        task.Notes.Should().Be(notesUpdateDetails);
        task.Status.Should().Be(statusUpdateDetails);
        task.Priority.Should().Be(priorityUpdateDetails);
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

        var updatedValidTitleWithinLimit = new string('a', titleLength);

        // When
        task.UpdateDetails(updatedValidTitleWithinLimit, ValidNotes, ValidUpdateTime);

        // Then
        task.Title.Should().Be(updatedValidTitleWithinLimit);
    }

    [Fact] // ensures trimming occurs before storing and validating title length
    public void UpdateDetails_WhenTitleHasSpacesButTrimmedLengthIs100_SetsTrimmedTitle()
    {
        // Given
        var task = CreateValidTask();

        var updatedTrimmedValidTitle = "   " + new string('a', 100) + "   ";

        // When
        task.UpdateDetails(updatedTrimmedValidTitle, ValidNotes, ValidUpdateTime);

        // Then
        task.Title.Should().Be(new string('a', 100));
    }

    [Fact] // ensures trimming occurs before storing and validating notes length
    public void UpdateDetails_WhenNotesHasSpacesButTrimmedLengthIs500_SetsTrimmedNotes()
    {
        // Given
        var task = CreateValidTask();

        var updatedValidNotesWithSpaces = "   " + new string('a', 500) + "   ";

        // When
        task.UpdateDetails(ValidTitle, updatedValidNotesWithSpaces, ValidUpdateTime);

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

        var updatedValidNotesWithinLimit = new string('a', notesLength);

        // When
        task.UpdateDetails(ValidTitle, updatedValidNotesWithinLimit, ValidUpdateTime);

        // Then
        task.Notes.Should().Be(updatedValidNotesWithinLimit);
    }

    [Fact]
    public void UpdateDetails_WithValidTitleAndNotes_UpdatesTask()
    {
        // Given
        var task = CreateValidTask();

        // When
        task.UpdateDetails(UpdatedValidTitle, UpdatedValidNotes, ValidUpdateTime);

        // Then
        task.Title.Should().Be(UpdatedValidTitle);
        task.Notes.Should().Be(UpdatedValidNotes);
        task.UpdatedAt.Should().Be(ValidUpdateTime);
    }

    [Fact]
    public void UpdateDetails_WithValidTitleAndNotes_DoesNotModifyNonEditableFields()
    {
        // Given
        var task = CreateValidTask();

        var idBeforeUpdateDetails = task.Id;
        var statusUpdateDetails = task.Status;
        var createdAtUpdateDetails = task.CreatedAt;
        var completedAtUpdateDetails = task.CompletedAt;


        // When
        task.UpdateDetails(UpdatedValidTitle, UpdatedValidNotes, ValidUpdateTime);

        // Then
        task.Id.Should().Be(idBeforeUpdateDetails);
        task.Status.Should().Be(statusUpdateDetails);
        task.CreatedAt.Should().Be(createdAtUpdateDetails);
        task.CompletedAt.Should().Be(completedAtUpdateDetails);

    }

}