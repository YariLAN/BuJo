namespace BuJo.Domain.Habits;

public sealed class HabitLog
{
    private HabitLog(Guid id, Guid habitId, DateOnly date, bool isCompleted)
    {
        Id = id;
        HabitId = habitId;
        Date = date;
        IsCompleted = isCompleted;
    }

    public HabitLog() {}
    
    public Guid Id { get; private set; }
    
    public Guid HabitId { get; private set; }
    
    public DateOnly Date { get; private set; }
    
    public bool IsCompleted { get; private set; }

    public static HabitLog Create(Guid habitId, DateOnly date, bool isCompleted)
        => new(Guid.NewGuid(), habitId, date, isCompleted);
}