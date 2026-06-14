using Ardalis.Specification;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public sealed class GetHabitLogBySpecification : Specification<HabitLog>
{
    public GetHabitLogBySpecification(
        Guid? habitId = null, 
        Guid? userId = null,
        DateOnly? specificDate = null, 
        DateOnly? fromDate = null, 
        DateOnly? toDate = null)
    {
        Query.Take(10000);
        
        Query.Where(hl => hl.HabitId == habitId, habitId is not null);

        if (userId is not null)
        {
            Query.Include(hl => hl.Habit);
            Query.Where(hl => hl.Habit.UserId == userId);
        }
        
        Query.Where(hl => hl.Date == specificDate, specificDate is not null);

        Query.Where(hl => hl.Date >= fromDate && hl.Date <= toDate, fromDate is not null && toDate is not null);
        
        Query.Include(hl => hl.Habit, userId is not null);
    }
}