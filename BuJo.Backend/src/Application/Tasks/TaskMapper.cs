using BuJo.Contracts.V1.Tasks;
using Task = BuJo.Domain.Tasks.Task;

namespace BuJo.Application.Tasks;

internal static class TaskMapper
{
    public static TaskResponse ToResponse(this Task task) => TaskResponse.Create(
        task.Id,
        task.Title,
        task.Description,
        (TaskPriorityDto)task.Priority,
        (TaskStatusDto)task.Status,
        task.DueDateTime,
        task.CreatedAt);
}