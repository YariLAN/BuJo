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
}
