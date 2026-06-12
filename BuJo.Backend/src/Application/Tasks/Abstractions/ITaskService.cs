using BuJo.Contracts.V1.Tasks;

namespace BuJo.Application.Tasks.Abstractions;

public interface ITaskService
{
    Task<List<TaskResponse>> GetTasksAsync(Guid userId, CancellationToken ct);

    Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct);
}