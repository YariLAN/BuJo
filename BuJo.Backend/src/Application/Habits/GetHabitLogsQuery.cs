namespace BuJo.Application.Habits;

public sealed record GetHabitLogsQuery(
    Guid UserId, 
    Guid? HabitId = null, 
    DateOnly? FromDate = null, 
    DateOnly? ToDate = null);