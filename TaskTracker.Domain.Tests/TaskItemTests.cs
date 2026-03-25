namespace TaskTracker.Domain.Tests;
using FluentAssertions;

public class TaskItemTests
{
    // MethodName_WhenCondition_ExpectedResult

    // Create Tests
    // happy path tests
    [Fact]
    public void Create_WhenInputsAreValid_ReturnsTaskItem()
    {
        // Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var title = "Buy milk";
        var notes = "From the store";
        
        // When
        var task = TaskItem.Create(title, notes, now);
    
        // Then
        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("Buy milk");
        task.Notes.Should().Be("From the store");
        task.Status.Should().Be(TaskStatus.Active);
        task.CreatedAt.Should().Be(now);
        task.CompletedAt.Should().BeNull();
    }

    // defaults and state tests
    [Fact]
    public void Create_WhenCalled_SetsStatusToActive()
    {
        // Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var title = "Buy milk";
        var notes = "From the store";
        
        // When
        var task = TaskItem.Create(title, notes, now);
    
        // Then
        task.Status.Should().Be(TaskStatus.Active);
    }

    [Fact]
    public void Create_WhenCalled_SetsCompletedAtToNull()
    {
        // Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var title = "Buy milk";
        var notes = "From the store";
        
        // When
        var task = TaskItem.Create(title, notes, now);
    
        // Then
        task.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WhenCalled_SetsCreatedAtToNow()
    {
        // Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var title = "Buy milk";
        var notes = "From the store";
        
        // When
        var task = TaskItem.Create(title, notes, now);
    
        // Then
        task.CreatedAt.Should().Be(now);
    }

    
    [Fact]
    public void Create_WhenCalled_AssignsANewId()
    {
        // Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        var title = "Buy milk";
        var notes = "From the store";
        
        // When
        var task = TaskItem.Create(title, notes, now);
    
        // Then
        task.Id.Should().NotBeEmpty();
    }


    // validation tests
    [Fact]
    public void Create_WhenTitleIsNull_ThrowsArgumentException()
    {
        /// Given        
        var now = new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero);
        
        // When
        Action act = () => TaskItem.Create(null!, null, now);
    
        // Then
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Create_WhenTitleIsEmpty_ThrowsArgumentException()
    {
        // Given
    
        // When
    
        // Then
    }

    [Fact]
    public void Create_WhenTitleIsWhitespace_ThrowsArgumentException()
    {
        // Given
    
        // When
    
        // Then
    }

    [Fact]
    public void Create_WhenTitleExceeds100Characters_ThrowsArgumentExceptionTestName()
    {
        // Given
    
        // When
    
        // Then
    }


    [Fact]
    public void Create_WhenNotesExceed500Characters_ThrowsArgumentException()
    {
        // Given
    
        // When
    
        // Then
    }

    // normalization tests
    [Fact]
    public void Create_WhenTitleHasLeadingOrTrailingSpaces_TrimsTitle()
    {
        // Given
    
        // When
    
        // Then
    }

    [Fact]
    public void TesCreate_WhenNotesHasLeadingOrTrailingSpaces_TrimsNotestName()
    {
        // Given
    
        // When
    
        // Then
    }

    [Fact]
    public void Create_WhenNotesIsWhitespace_SetsNotesToEmptyOrNull()
    {
        // Given
    
        // When
    
        // Then
    }








    // Close Tests
    // Reopen Tests
}
