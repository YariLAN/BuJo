using Ardalis.Specification;
using Task = BuJo.Domain.Tasks.Task;

namespace BuJo.Application.Tasks.Abstractions;

public interface ITaskRepository : IRepositoryBase<Task>
{
    Task<List<Task>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    
    Task<Task?> GetBySpecAsync(ISpecification<Task> specification, CancellationToken ct);
}