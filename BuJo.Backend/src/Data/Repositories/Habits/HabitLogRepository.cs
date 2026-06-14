using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using BuJo.Application.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Data.Repositories.Habits;

public sealed class HabitLogRepository : RepositoryBase<HabitLog>, IHabitLogRepository
{
    public HabitLogRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public Task<List<HabitLog>> ListBySpecAsync(ISpecification<HabitLog> specification, CancellationToken ct)
        => ListAsync(specification, ct);

    public Task<HabitLog?> FirstOrDefaultBySpecAsync(ISpecification<HabitLog> specification, CancellationToken ct)
        => FirstOrDefaultAsync(specification, ct);
}