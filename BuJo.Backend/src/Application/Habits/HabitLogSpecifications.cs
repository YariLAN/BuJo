using Ardalis.Specification;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public sealed class HabitLogByHabitSpec : Specification<HabitLog>
{
    public HabitLogByHabitSpec(Guid habitId)
    {
        Query.Where(hl => hl.HabitId == habitId);
    }
}

public sealed class HabitLogByDateSpec : Specification<HabitLog>
{
    public HabitLogByDateSpec(DateOnly date)
    {
        Query.Where(hl => hl.Date == date);
    }
}

public sealed class HabitLogByHabitAndDateSpec : Specification<HabitLog>
{
    public HabitLogByHabitAndDateSpec(Guid habitId, DateOnly date)
    {
        Query.Where(hl => hl.HabitId == habitId && hl.Date == date);
    }
}

public sealed class HabitLogByHabitDateRangeSpec : Specification<HabitLog>
{
    public HabitLogByHabitDateRangeSpec(Guid habitId, DateOnly fromDate, DateOnly toDate)
    {
        Query.Where(hl => hl.HabitId == habitId && hl.Date >= fromDate && hl.Date <= toDate);
    }
}

public sealed class HabitLogByUserDateRangeSpec : Specification<HabitLog>
{
    public HabitLogByUserDateRangeSpec(Guid userId, DateOnly fromDate, DateOnly toDate)
    {
        Query.Where(hl => hl.Habit!.UserId == userId && hl.Date >= fromDate && hl.Date <= toDate);
        Query.Include(hl => hl.Habit);
    }
}

public sealed class HabitLogByUserSpec : Specification<HabitLog>
{
    public HabitLogByUserSpec(Guid userId)
    {
        Query.Where(hl => hl.Habit!.UserId == userId);
        Query.Include(hl => hl.Habit);
    }
}

public sealed class HabitLogByUserHabitSpec : Specification<HabitLog>
{
    public HabitLogByUserHabitSpec(Guid userId, Guid habitId)
    {
        Query.Where(hl => hl.Habit!.UserId == userId && hl.HabitId == habitId);
        Query.Include(hl => hl.Habit);
    }
}