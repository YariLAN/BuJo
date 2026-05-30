namespace BuJo.Contracts.V1.Accounting;

public sealed class UserResponse
{
    public Guid? Id { get; init; }
    
    public string? TelegramId { get; init; }
    
    public string? Name { get; init; }
    
    public TimeOnly? ReminderMorningTime { get; init; }
    
    public TimeOnly? ReminderEveningTime { get; init; }
    
    public static UserResponse Create(
        Guid? id,
        string? telegramId,
        string? name,
        TimeOnly? reminderMorningTime,
        TimeOnly? reminderEveningTime) => new()
    {
        Id = id,
        TelegramId = telegramId,
        Name = name,
        ReminderMorningTime = reminderMorningTime,
        ReminderEveningTime = reminderEveningTime,
    };
}