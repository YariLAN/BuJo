using Ardalis.Specification;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

public interface IHabitLogRepository : IRepositoryBase<HabitLog>
{
    Task<List<HabitLog>> ListBySpecAsync(ISpecification<HabitLog> specification, CancellationToken ct);

    Task<HabitLog?> FirstOrDefaultBySpecAsync(ISpecification<HabitLog> specification, CancellationToken ct);
}