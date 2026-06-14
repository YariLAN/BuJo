namespace BuJo.Contracts.V1.Habits;

public sealed record LogHabitRequest(string? Date, bool? IsCompleted);