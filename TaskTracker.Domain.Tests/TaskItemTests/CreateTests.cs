using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Domain.Tests.TaskItemTests;

public class CreateTests : TaskItemTestBase
{
    [Fact] // create requires a real timestamp and rejects default value
    public void Create_WhenTimeIsDefault_ThrowsDomainValidationException()
    {
        // When
        Action act = () => TaskItem.Create(ValidTitle, ValidNotes, ValidPriority, DefaultTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("now").WithMessage("*Now must be a valid time.*");
    }

    [Fact] // unkown priority throws exception
    public void Create_WithUnknownPriority_ThrowsDomainValidationException()
    {
        // Given
        var unknownPriority = TaskPriority.Unknown;

        // When
        Action act = () => TaskItem.Create(ValidTitle, ValidNotes, unknownPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("priority").WithMessage("*Priority must be Low, Medium, High, or Critical.*");
    }

    [Fact] // undefined prioirty throws exception
    public void Create_WithUndefinedPriority_ThrowsDomainValidationException()
    {
        // Given
        var undefinedPriority = (TaskPriority)999;

        // When
        Action act = () => TaskItem.Create(ValidTitle, ValidNotes, undefinedPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("priority").WithMessage("*Priority must be Low, Medium, High, or Critical.*");
    }

    [Theory] // valid priority creates task and sets prioirty
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.Medium)]
    [InlineData(TaskPriority.High)]
    [InlineData(TaskPriority.Critical)]
    public void Create_WithValidPriority_SetsPriority(TaskPriority priority)
    {
        // When
        var task = TaskItem.Create(ValidTitle, ValidNotes, priority, ValidCreateTime);

        // Then
        task.Priority.Should().Be(priority);
    }


    [Theory] // title required and cannot be null empty or whitespace
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces
    public void Create_WhenTitleIsNullEmptyOrWhitespace_ThrowsDomainValidationException(string? title)
    {
        // When
        Action act = () => TaskItem.Create(title!, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot be null, empty, or whitespace.*");
    }

    [Fact] // guards against validating raw title length instead of trimmed length
    public void Create_WhenTrimmedTitleLengthExceedsLimit_ThrowsDomainValidationException()
    {
        // Given
        // surrounding spaces should not make an over-limit title pass validation
        var invalidTitleLengthAfterTrim = "   " + new string('a', 101) + "   ";

        // When
        Action act = () => TaskItem.Create(invalidTitleLengthAfterTrim, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot exceed 100 characters.*");
    }

    [Theory] // title must not exceed 100 characters after normalization
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public void Create_WhenTitleLengthExceedsLimit_ThrowsDomainValidationException(int titleLength)
    {
        // Given
        var titleWithInvalidLength = new string('a', titleLength);

        // When
        Action act = () => TaskItem.Create(titleWithInvalidLength, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("title").WithMessage("*Title cannot exceed 100 characters.*");
    }

    [Theory] // title lengths within the allowed limit should be accepted
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Create_WhenTitleLengthIsWithinLimit_SetsTitle(int titleLength)
    {
        // Given
        var validTitle = new string('a', titleLength);

        // When
        var task = TaskItem.Create(validTitle, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        task.Title.Should().Be(validTitle);
    }

    [Fact] // ensures trimming occurs before storing and validating title length
    public void Create_WhenTitleHasSpacesButTrimmedLengthIs100_SetsTrimmedTitle()
    {
        // Given
        var validTitleWithSpaces = "   " + new string('a', 100) + "   ";

        // When
        var task = TaskItem.Create(validTitleWithSpaces, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        task.Title.Should().Be(new string('a', 100));
    }

    [Fact] // guards against validating raw notes length instead of trimmed length
    public void Create_WhenTrimmedNotesLengthExceedsLimit_ThrowsDomainValidationException()
    {
        // Given
        var invalidNotesLengthAfterTrim = "   " + new string('a', 501) + "   ";

        // When
        Action act = () => TaskItem.Create(ValidTitle, invalidNotesLengthAfterTrim, ValidPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("notes").WithMessage("*Notes cannot exceed 500 characters.*");
    }

    [Theory] // notes must not exceed 500 characters after normalization
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public void Create_WhenNotesLengthExceedsLimit_ThrowsDomainValidationException(int notesLength)
    {
        // Given
        var notesWithInvalidLength = new string('a', notesLength);

        // When
        Action act = () => TaskItem.Create(ValidTitle, notesWithInvalidLength, ValidPriority, ValidCreateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("notes").WithMessage("*Notes cannot exceed 500 characters.*");
    }

    [Theory] // notes are normalized to null instead of being stored as empty text
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    [InlineData("     ")] // moar whitespaces
    public void Create_WhenNotesIsNullEmptyOrWhitespace_SetsNotesToNull(string? notes)
    {
        // When
        var task = TaskItem.Create(ValidTitle, notes, ValidPriority, ValidCreateTime);

        // Then
        task.Notes.Should().BeNull();
    }

    [Fact] // ensures trimming occurs before storing and validating notes length
    public void Create_WhenNotesHasSpacesButTrimmedLengthIs500_SetsTrimmedNotes()
    {
        // Given
        var validNotesWithSpaces = "   " + new string('a', 500) + "   ";

        // When
        var task = TaskItem.Create(ValidTitle, validNotesWithSpaces, ValidPriority, ValidCreateTime);

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
        var notesWithValidLength = new string('a', notesLength);

        // When
        var task = TaskItem.Create(ValidTitle, notesWithValidLength, ValidPriority, ValidCreateTime);

        // Then
        task.Notes.Should().Be(notesWithValidLength);
    }

    [Fact] // valid inputs should produce a fully initialized active task
    public void Create_WhenInputsAreValid_SetsAllProperties()
    {
        // When
        var task = TaskItem.Create(ValidTitle, ValidNotes, ValidPriority, ValidCreateTime);

        // Then       

        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be(ValidTitle);
        task.Notes.Should().Be(ValidNotes);
        task.Status.Should().Be(TaskStatus.Active);
        task.Priority.Should().Be(ValidPriority);
        task.CreatedAt.Should().Be(ValidCreateTime);
        task.CompletedAt.Should().BeNull();
        task.UpdatedAt.Should().BeNull();
    }

    [Fact] // each created task should receive a unique identifier
    public void Create_WhenCalled_AssignsDifferentIds()
    {
        // When
        var task1 = TaskItem.Create(ValidTitle, ValidNotes, ValidPriority, ValidCreateTime);
        var task2 = TaskItem.Create(ValidTitle, ValidNotes, ValidPriority, ValidCreateTime);

        // Then
        task1.Id.Should().NotBeEmpty();
        task2.Id.Should().NotBeEmpty();
        task1.Id.Should().NotBe(task2.Id);
    }

}