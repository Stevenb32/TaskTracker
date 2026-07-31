using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Domain.Tests.TaskItemTests;

public class UpdatePriorityTests : TaskItemTestBase
{
    [Theory]
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.High)]
    [InlineData(TaskPriority.Critical)]
    public void ChangePriority_WithValidPriority_ChangesPriorityAndUpdatedAt(TaskPriority priority)
    {
        // Given
        var task = CreateValidTask(priority: TaskPriority.Medium);

        // When
        task.ChangePriority(priority, ValidUpdateTime);

        // Then
        task.Priority.Should().Be(priority);
        task.UpdatedAt.Should().Be(ValidUpdateTime);
    }

    [Fact]
    public void ChangePriority_WithSamePriority_DoesNotChangeUpdatedAt()
    {
        // Given
        var task = CreateValidTask(priority: TaskPriority.Medium);

        // When
        task.ChangePriority(TaskPriority.Medium, ValidUpdateTime);

        // Then
        task.Priority.Should().Be(TaskPriority.Medium);
        task.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(TaskPriority.Unknown)]
    [InlineData((TaskPriority)999)]
    public void ChangePriority_WithInvalidPriority_ThrowsAndPreservesState(TaskPriority invalidPriority)
    {
        // Given
        var task = CreateValidTask(priority: TaskPriority.Medium);

        // When
        Action act = () => task.ChangePriority(invalidPriority, ValidUpdateTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("priority").WithMessage("*Priority must be Low, Medium, High, or Critical.*");

        task.Priority.Should().Be(TaskPriority.Medium);
        task.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void ChangePriority_WhenTimeIsDefault_ThrowsAndPreservesState()
    {
        // Given
        var task = CreateValidTask(priority: TaskPriority.Medium);

        // When
        Action act = () => task.ChangePriority(TaskPriority.Critical, DefaultTime);

        // Then
        act.Should().Throw<DomainValidationException>().WithParameterName("now").WithMessage("*Now must be a valid time.*");

        task.Priority.Should().Be(TaskPriority.Medium);
        task.UpdatedAt.Should().BeNull();
    }

}