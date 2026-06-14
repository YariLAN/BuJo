using Ardalis.Specification;
using BuJo.Application.Habits;
using BuJo.Domain.Habits;
using Moq;
using Xunit;

namespace BuJo.Application.Tests.Services;

public sealed class HabitServiceTests : TestsBase
{
    private readonly HabitService _sut;

    public HabitServiceTests()
    {
        _sut = new HabitService(HabitRepositoryMock.Object, HabitLogRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsValid_CreatesAndReturnsResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateHabitCommand(userId, "Читать книги");

        HabitRepositoryMock
            .Setup(r => r.AnyBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        HabitRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Habit h, CancellationToken _) => h);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Читать книги", result.Name);
        Assert.False(result.IsArchived);

        HabitRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Habit>(h => h.UserId == userId && h.Name == "Читать книги"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_TrimsNameBeforeSaving()
    {
        // Arrange
        var command = new CreateHabitCommand(Guid.NewGuid(), "  Зарядка  ");

        HabitRepositoryMock
            .Setup(r => r.AnyBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        HabitRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Habit h, CancellationToken _) => h);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("Зарядка", result.Name);

        HabitRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Habit>(h => h.Name == "Зарядка"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenNameIsEmptyOrWhitespace_ThrowsArgumentException(string name)
    {
        // Arrange
        var command = new CreateHabitCommand(Guid.NewGuid(), name);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        HabitRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenNameTooLong_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateHabitCommand(Guid.NewGuid(), new string('a', 101));

        HabitRepositoryMock
            .Setup(r => r.AnyBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        HabitRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDuplicateExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CreateHabitCommand(Guid.NewGuid(), "Медитация");

        HabitRepositoryMock
            .Setup(r => r.AnyBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        Assert.Contains("Медитация", exception.Message);

        HabitRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Habit>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetListAsync_ReturnsMappedResponses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var habits = new List<Habit>
        {
            Habit.Create(userId, "Читать книги"),
            Habit.Create(userId, "Зарядка"),
        };

        HabitRepositoryMock
            .Setup(r => r.ListBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(habits);

        // Act
        var result = await _sut.GetListAsync(new GetHabitsQuery(userId), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Читать книги", result[0].Name);
        Assert.Equal("Зарядка", result[1].Name);
    }

    [Fact]
    public async Task GetListAsync_WhenNoHabits_ReturnsEmptyList()
    {
        // Arrange
        HabitRepositoryMock
            .Setup(r => r.ListBySpecAsync(It.IsAny<ISpecification<Habit>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Habit>());

        // Act
        var result = await _sut.GetListAsync(new GetHabitsQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
