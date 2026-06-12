using AutoFixture;
using AutoFixture.AutoMoq;
using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Application.Tasks.Abstractions;
using Moq;

namespace BuJo.Application.Tests;

public abstract class TestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());
    
    protected readonly Mock<IUserRepository> UserRepositoryMock;
    protected readonly Mock<IUserBotStateRepository> UserBotStateRepositoryMock;
    
    protected readonly Mock<IUserService> UserServiceMock;
    protected readonly Mock<IUserBotStateService> UserBotStateServiceMock;

    protected readonly Mock<ITaskRepository> TaskRepositoryMock;

    protected TestsBase()
    {
        CustomizeEntities();

        UserRepositoryMock = new Mock<IUserRepository>();
        UserBotStateRepositoryMock = new Mock<IUserBotStateRepository>();
        UserServiceMock = new Mock<IUserService>();
        UserBotStateServiceMock = new Mock<IUserBotStateService>();
        
        TaskRepositoryMock = new Mock<ITaskRepository>();
    }

    /// <summary>
    /// Настройка AutoFixture для создания доменных сущностей через фабричные методы,
    /// т.к. их конструкторы приватные или параметризованные.
    /// </summary>
    private void CustomizeEntities()
    {
        
    }
}