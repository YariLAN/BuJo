using BuJo.Domain.Accounting;

namespace BuJo.Domain.Tasks;

public sealed class Task
{
    private User? _user;
    
    private Task(Guid id, Guid userId, string title, TaskStatus status, DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Status = status;
        CreatedAt = createdAt;
    }
    
    public Task() {}

    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public User User
    {
        get => _user ?? throw new ArgumentNullException($"Пользователь не может быть null");
        private set => _user = value;
    }
    
    public string Title { get; private set; }
    
    public string? Description { get; private set; }
    
    public TaskStatus Status { get; private set; }
    
    /// <summary>
    /// Дата-время начала задачи
    /// </summary>
    public DateTimeOffset? StartDateTime { get; private set; }
    
    /// <summary>
    /// Дата-время конца задачи или дэдлайн
    /// </summary>
    public DateTimeOffset? DueDateTime { get; private set; }
    
    /// <summary>
    /// Дата-время напоминания о задаче
    /// </summary>
    public DateTimeOffset? ReminderAt { get; private set; }
    
    /// <summary>
    /// Было ли напоминание о задаче
    /// </summary>
    public bool IsSentReminder { get; private set; } 
    
    public DateTimeOffset CreatedAt { get; private set; }

    public void ChangeStatus(TaskStatus status) => Status = status;
    
    public static Task Create(
        Guid userId, 
        string title, 
        string? description = null, 
        DateTimeOffset? startDate = null, 
        DateTimeOffset? dueDate = null,
        DateTimeOffset? reminderAt = null)
        => new(Guid.NewGuid(), userId, title, TaskStatus.ToDo, DateTimeOffset.Now)
        {
            Description = description, 
            StartDateTime = startDate,
            DueDateTime = dueDate,
            ReminderAt = reminderAt,
        };
}