using BuJo.Domain.Habits;

namespace BuJo.Domain.Accounting;

/// <summary>
/// Пользователь
/// </summary>
public sealed class User
{
    private List<Habit> _habits = [];
    private List<Tasks.Task> _tasks = [];
    
    private User(Guid id, string telegramId, string? name, DateTimeOffset createdAt)
    {
        Id = id;
        TelegramId = telegramId;
        Name = name;
        CreatedAt = createdAt;
    }

    public User() {}

    public Guid Id { get; private set; }

    public string TelegramId { get; private set; }

    public string? Name { get; private set; }
    
    public TimeOnly? ReminderMorningTime { get; private set; }
    
    public TimeOnly? ReminderEveningTime { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<Habit> Habits
    {
        get => _habits;
        private set => _habits = value.ToList();
    }

    public IReadOnlyList<Tasks.Task> Tasks
    {
        get => _tasks;
        private set => _tasks = value.ToList();
    }

    public void SetReminder(ReminderKind kind, TimeOnly time)
    {
        switch (kind)
        {
            case ReminderKind.Morning:
                ReminderMorningTime = time;
                break;
            case ReminderKind.Evening:
                ReminderEveningTime = time;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "Неизвестный тип напоминания");
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static User Create(string telegramId, string? name)
    {
        var now = DateTimeOffset.UtcNow;
        return new User(Guid.NewGuid(), telegramId, name, now)
        {
            UpdatedAt = now
        };
    }
}