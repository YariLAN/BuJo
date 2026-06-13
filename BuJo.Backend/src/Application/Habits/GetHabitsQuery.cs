namespace BuJo.Application.Habits;

public sealed record GetHabitsQuery(
    Guid UserId, 
    bool IncludeArchived = false, 
    string? Name = null);
