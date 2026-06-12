namespace BuJo.Contracts.V1.Tasks;

public sealed class TaskResponse
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public TaskPriorityDto Priority { get; init; }
    
    public TaskStatusDto Status { get; init; }
    
    public DateTimeOffset? DueDate { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public static TaskResponse Create(
        Guid id,
        string title,
        string? description,
        TaskPriorityDto priority,
        TaskStatusDto status,
        DateTimeOffset? dueDate,
        DateTimeOffset createdAt) => new()
    {
        Id = id,
        Title = title,
        Description = description,
        Priority = priority,
        Status = status,
        DueDate = dueDate,
        CreatedAt = createdAt,
    };
}