using BuJo.Application.Tasks.Abstractions;
using BuJo.Contracts.V1.Tasks;
using BuJo.Domain.Tasks;
using Microsoft.Extensions.Logging;
using Task = BuJo.Domain.Tasks.Task;

namespace BuJo.Application.Tasks;

internal sealed class TaskService(
    ITaskRepository taskRepository,
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<List<TaskResponse>> GetTasksAsync(Guid userId, CancellationToken ct)
    {
        logger.LogInformation("Getting tasks for user {UserId}", userId);
        
        var tasks = await taskRepository.GetByUserIdAsync(userId, ct);
        
        return tasks
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDateTime)
            .Select(t => t.ToResponse())
            .ToList();
    }

    public async Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required", nameof(request.Title));
        
        if (request.Title.Length > 500)
            throw new ArgumentException("Title must be at most 500 characters", nameof(request.Title));
        
        var priority = request.Priority is not null
            ? (TaskPriority)request.Priority
            : TaskPriority.Medium;

        var task = Task.Create(
            userId,
            request.Title.Trim(),
            request.Description?.Trim(),
            priority,
            dueDate: request.DueDate!.Value.UtcDateTime);

        await taskRepository.AddAsync(task, ct);
        
        logger.LogInformation("Created task {TaskId} for user {UserId}", task.Id, userId);
        
        return task.ToResponse();
    }
}