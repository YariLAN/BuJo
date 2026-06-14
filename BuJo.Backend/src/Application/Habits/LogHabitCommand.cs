namespace BuJo.Application.Habits;

public sealed record LogHabitCommand(Guid UserId, Guid HabitId, DateOnly Date, bool IsCompleted);