using BuJo.Contracts.V1.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public static class HabitMapper
{
    public static HabitResponse ToResponse(this Habit habit) => HabitResponse.Create(
        habit.Id,
        habit.Name,
        habit.IsArchived,
        habit.CreatedAt);

    public static HabitLogResponse ToResponse(this HabitLog log) => HabitLogResponse.Create(
        log.Id,
        log.HabitId,
        log.Habit.Name,
        log.Date,
        log.IsCompleted);

    public static HabitLogResponse ToResponse(this HabitLog log, string? habitName) => HabitLogResponse.Create(
        log.Id,
        log.HabitId,
        habitName,
        log.Date,
        log.IsCompleted);
}
