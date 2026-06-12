using AutoFixture;
using BuJo.Application.Tasks;
using BuJo.Contracts.V1.Tasks;
using BuJo.Domain.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using DomainTask = BuJo.Domain.Tasks.Task;

namespace BuJo.Application.Tests.Services;

public sealed class TaskServiceTests : TestsBase
{
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _sut = new TaskService(TaskRepositoryMock.Object, NullLogger<TaskService>.Instance);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasksAsync_WhenTasksExist_ReturnsOrderedTaskList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<DomainTask>
        {
            DomainTask.Create(userId, "Low priority", priority: TaskPriority.Low),
            DomainTask.Create(userId, "Critical priority", priority: TaskPriority.Critical),
            DomainTask.Create(userId, "Medium priority", priority: TaskPriority.Medium),
        };

        TaskRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetTasksAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        // Ordered by Priority desc: Critical → Medium → Low
        Assert.Equal(TaskPriorityDto.Critical, result[0].Priority);
        Assert.Equal(TaskPriorityDto.Medium, result[1].Priority);
        Assert.Equal(TaskPriorityDto.Low, result[2].Priority);

        TaskRepositoryMock.Verify(
            r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasksAsync_WhenNoTasks_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        TaskRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DomainTask>());

        // Act
        var result = await _sut.GetTasksAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        TaskRepositoryMock.Verify(
            r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTasksAsync_OrdersByDueDateWhenPriorityIsSame()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dueLater = DateTimeOffset.Now.AddDays(3);
        var dueSoon  = DateTimeOffset.Now.AddDays(1);

        var tasks = new List<DomainTask>
        {
            DomainTask.Create(userId, "Later", priority: TaskPriority.Medium, dueDate: dueLater),
            DomainTask.Create(userId, "Sooner", priority: TaskPriority.Medium, dueDate: dueSoon),
        };

        TaskRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetTasksAsync(userId, CancellationToken.None);

        // Assert — sooner due date comes first when priority is equal
        Assert.Equal("Sooner", result[0].Title);
        Assert.Equal("Later", result[1].Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithAllFields_CreatesAndReturnsTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTaskRequest(
            Title: "Test Task",
            Description: "Test Description",
            Priority: TaskPriorityDto.High,
            DueDate: DateTimeOffset.Now.AddDays(7));

        TaskRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainTask t, CancellationToken _) => t);

        // Act
        var result = await _sut.CreateAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.Priority, result.Priority);
        Assert.Equal(request.DueDate, result.DueDate);
        Assert.Equal(TaskStatusDto.ToDo, result.Status);

        TaskRepositoryMock.Verify(
            r => r.AddAsync(It.Is<DomainTask>(t =>
                t.Title == request.Title &&
                t.Description == request.Description &&
                t.Priority == TaskPriority.High &&
                t.DueDateTime == request.DueDate &&
                t.UserId == userId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithOnlyRequiredFields_UsesDefaults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTaskRequest(
            Title: "Minimal Task",
            Description: null,
            Priority: null,
            DueDate: null);

        TaskRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainTask t, CancellationToken _) => t);

        // Act
        var result = await _sut.CreateAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Null(result.Description);
        Assert.Equal(TaskPriorityDto.Medium, result.Priority); // default
        Assert.Null(result.DueDate);

        TaskRepositoryMock.Verify(
            r => r.AddAsync(It.Is<DomainTask>(t =>
                t.Priority == TaskPriority.Medium &&
                t.Description == null &&
                t.DueDateTime == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTaskRequest(
            Title: "",
            Description: null,
            Priority: null,
            DueDate: null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateAsync(userId, request, CancellationToken.None));

        Assert.Contains("Title", exception.Message);

        TaskRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_WithTitleTooLong_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var longTitle = new string('A', 501);
        var request = new CreateTaskRequest(
            Title: longTitle,
            Description: null,
            Priority: null,
            DueDate: null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateAsync(userId, request, CancellationToken.None));

        Assert.Contains("Title", exception.Message);

        TaskRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<DomainTask>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}