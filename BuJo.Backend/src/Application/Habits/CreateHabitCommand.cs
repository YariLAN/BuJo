namespace BuJo.Application.Habits;

public sealed record CreateHabitCommand(Guid UserId, string Name);
