using AutoFixture;
using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using Moq;
using Xunit;

namespace BuJo.Application.Tests.Services;

public sealed class UserServiceTests : TestsBase
{
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(UserRepositoryMock.Object);
    }

    [Fact]
    public async Task GetOrDefaultAsync_WhenUserExists_ReturnsUserResponse()
    {
        // Arrange
        var user = AutoFixture.Create<User>();
        var query = AutoFixture.Create<GetUserQuery>();

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetOrDefaultAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.TelegramId, result.TelegramId);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.ReminderMorningTime, result.ReminderMorningTime);
        Assert.Equal(user.ReminderEveningTime, result.ReminderEveningTime);

        UserRepositoryMock.Verify(
            r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrDefaultAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = AutoFixture.Create<GetUserQuery>();

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetOrDefaultAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);

        UserRepositoryMock.Verify(
            r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_CreatesAndReturnsUserResponse()
    {
        // Arrange
        var command = new CreateUserCommand(
            TelegramId: "123456789",
            Username: "testuser");

        var user = User.Create(command.TelegramId, command.Username);

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        UserRepositoryMock
            .Setup(r => r.AddAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.CreateAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.TelegramId, result.TelegramId);

        UserRepositoryMock.Verify(
            r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()),
            Times.Once);

        UserRepositoryMock.Verify(
            r => r.AddAsync(It.Is<User>(u =>
                u.TelegramId == command.TelegramId &&
                u.Name == command.Username),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CreateUserCommand(
            TelegramId: "123456789",
            Username: "testuser");

        var existingUser = AutoFixture.Create<User>();

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.CreateAsync(command, CancellationToken.None));

        Assert.Contains(command.TelegramId, exception.Message);

        UserRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SetReminderAsync_WhenUserExists_SetsReminderAndUpdates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var reminderKind = ReminderKind.Morning;
        var reminderTime = new TimeOnly(8, 0);
        var user = AutoFixture.Build<User>()
            .With(u => u.Id, userId)
            .Create();

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        UserRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.SetReminderAsync(userId, reminderKind, reminderTime, CancellationToken.None);

        // Assert
        Assert.Equal(reminderTime, user.ReminderMorningTime);

        UserRepositoryMock.Verify(
            r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()),
            Times.Once);

        UserRepositoryMock.Verify(
            r => r.UpdateAsync(user, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetReminderAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var reminderKind = ReminderKind.Morning;
        var reminderTime = new TimeOnly(8, 0);

        UserRepositoryMock
            .Setup(r => r.GetBySpecAsync(It.IsAny<GetUserSpecification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.SetReminderAsync(userId, reminderKind, reminderTime, CancellationToken.None));

        Assert.Contains(userId.ToString(), exception.Message);

        UserRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}