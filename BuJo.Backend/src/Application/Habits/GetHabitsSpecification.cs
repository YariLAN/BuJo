using Ardalis.Specification;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public sealed class GetHabitsSpecification : Specification<Habit>
{
    public GetHabitsSpecification(GetHabitsQuery query)
    {
        Query.Where(h => h.UserId == query.UserId);

        Query.Where(h => !h.IsArchived, !query.IncludeArchived);

        Query.Where(h => h.Name == query.Name, query.Name is not null);

        Query.OrderBy(h => h.CreatedAt);
    }
}
