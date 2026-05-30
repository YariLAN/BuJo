namespace BuJo.Application.Accounting;

public sealed record GetUserQuery(
    Guid? UserId, 
    string? TelegramId = null, 
    string? UserName = null);