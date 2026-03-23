namespace TaskTracker.Domain.Tests;
using FluentAssertions;

public class TaskItemTests
{
// constructor tests
    [Fact] // complete
    public void Constructor_WhenTitleIsValid_SetsStatusActiveAndCompletedAtNull()
    {
        // Given / Arrange
        var now = DateTimeOffset.UtcNow;

        // When / Act
        var task = TaskItem.Create("Buy milk", null, now);

        // Then / Assert
        task.Title.Should().Be("Buy milk");
        task.CreatedAt.Should().Be(now);
        task.Status.Should().Be(TaskStatus.Active);
        task.CompletedAt.Should().BeNull();
    }

    [Fact] // complete
    public void Constructor_WhenTitleIsNull_ThrowsArgumentException()
    {
        // Given
        var now = DateTimeOffset.UtcNow;
        string? title = null;
        // When
        Action act = () => new TaskItem(title!, null, now);
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }  

    [Fact] // complete
    public void Constructor_WhenTitleIsWhitespace_ThrowsArgumentException()
    {
        // Given
        var now = DateTimeOffset.UtcNow;
        // When
        Action act = () => new TaskItem(" ", null, now);
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact] // complete
    public void Constructor_WhenTitleIsEmpty_ThrowsArgumentException()
    {
        // Given
        var now = DateTimeOffset.UtcNow;
        string title = "";
        // When
        Action act = () => new TaskItem(title, null, now);
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }




// complete tests
    [Fact] // in progress
    public void Complete_WhenTaskIsActive_SetsStatusCompletedAndCompletedAt()
    {
        // Given
    
        // When
    
        // Then
    }

    // end of tests
}
