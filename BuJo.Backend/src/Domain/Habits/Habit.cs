using BuJo.Domain.Accounting;

namespace BuJo.Domain.Habits;

public sealed class Habit
{
    private User? _user;
    
    private Habit(Guid id, Guid userId, string name, bool isArchived, DateTimeOffset createdAt)
    {
        UserId = userId;
        Name = name;
        IsArchived = isArchived;
        CreatedAt = createdAt;
        Id = id;
    }

    public Guid Id { get; private set; }
    
    public string Name { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public bool IsArchived { get; private set; }
    
    public DateTimeOffset CreatedAt { get; private set; }
    
    public DateTimeOffset UpdatedAt { get; private set; }

    public User User
    {
        get => throw new ArgumentNullException($"Пользователь не может быть null");
        private set => _user = value;
    }

    public static Habit Create(Guid userId, string name)
    {
        var now = DateTimeOffset.Now;
        return new Habit(Guid.NewGuid(), userId, name, false, now)
        {
            UpdatedAt = now
        };
    }
}