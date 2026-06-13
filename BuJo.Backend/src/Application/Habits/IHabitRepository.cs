using Ardalis.Specification;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public interface IHabitRepository : IRepositoryBase<Habit>
{
    Task<List<Habit>> ListBySpecAsync(ISpecification<Habit> specification, CancellationToken ct);

    Task<bool> AnyBySpecAsync(ISpecification<Habit> specification, CancellationToken ct);
}
