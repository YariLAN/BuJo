namespace BuJo.Application.Accounting;

public sealed record CreateUserCommand(string TelegramId, string? Username = null);