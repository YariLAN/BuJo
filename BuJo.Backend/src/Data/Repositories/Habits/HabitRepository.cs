using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using BuJo.Application.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Data.Repositories.Habits;

public class HabitRepository : RepositoryBase<Habit>, IHabitRepository
{
    public HabitRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public Task<List<Habit>> ListBySpecAsync(ISpecification<Habit> specification, CancellationToken ct)
        => ListAsync(specification, ct);

    public Task<bool> AnyBySpecAsync(ISpecification<Habit> specification, CancellationToken ct)
        => AnyAsync(specification, ct);
}
