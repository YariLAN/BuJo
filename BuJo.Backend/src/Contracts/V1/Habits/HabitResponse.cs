namespace BuJo.Contracts.V1.Habits;

public sealed class HabitResponse
{
    public Guid? Id { get; init; }

    public string? Name { get; init; }

    public bool? IsArchived { get; init; }

    public DateTimeOffset? CreatedAt { get; init; }

    public static HabitResponse Create(
        Guid id,
        string name,
        bool isArchived,
        DateTimeOffset createdAt) => new()
    {
        Id = id,
        Name = name,
        IsArchived = isArchived,
        CreatedAt = createdAt,
    };
}
