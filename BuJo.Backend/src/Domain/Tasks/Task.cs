using BuJo.Domain.Accounting;

namespace BuJo.Domain.Tasks;

public sealed class Task
{
    private User? _user;
    
    public Task(Guid id, Guid userId, string title, TaskStatus status, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User
    {
        get => throw new ArgumentNullException($"Пользователь не может быть null");
        private set => _user = value;
    }
    
    public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    public TaskStatus Status { get; private set; }
    
    public DateTimeOffset? DueDateTime { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }

    public void ChangeStatus(TaskStatus status) => Status = status;
    
    public static Task Create(Guid userId, string title, string? description, DateTimeOffset? dueDate)
        => new(Guid.NewGuid(), userId, title, TaskStatus.ToDo, DateTimeOffset.Now)
        {
            Description = description, 
            DueDateTime = dueDate,
        };
}