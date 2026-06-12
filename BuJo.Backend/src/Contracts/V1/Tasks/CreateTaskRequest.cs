namespace BuJo.Contracts.V1.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriorityDto? Priority,
    DateTimeOffset? DueDate);