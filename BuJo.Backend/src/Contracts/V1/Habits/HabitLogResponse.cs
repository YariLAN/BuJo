namespace BuJo.Contracts.V1.Habits;

public sealed class HabitLogResponse
{
    public Guid Id { get; init; }
    
    public Guid HabitId { get; init; }
    
    public string? HabitName { get; init; }
    
    public DateOnly Date { get; init; }
    
    public bool IsCompleted { get; init; }

    public static HabitLogResponse Create(Guid id, Guid habitId, string? habitName, DateOnly date, bool isCompleted) => new()
    {
        Id = id,
        HabitId = habitId,
        HabitName = habitName,
        Date = date,
        IsCompleted = isCompleted,
    };
}