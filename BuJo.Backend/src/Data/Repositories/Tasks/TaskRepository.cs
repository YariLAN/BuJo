using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using BuJo.Application.Tasks.Abstractions;
using Microsoft.EntityFrameworkCore;
using Task = BuJo.Domain.Tasks.Task;

namespace BuJo.Data.Repositories.Tasks;

public class TaskRepository : RepositoryBase<Task>, ITaskRepository
{
    private readonly DataContext _dataContext;

    public TaskRepository(DataContext dataContext) : base(dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Task>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _dataContext.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync(ct);
    }

    public Task<Task?> GetBySpecAsync(ISpecification<Task> specification, CancellationToken ct)
        => FirstOrDefaultAsync(specification, ct);
}