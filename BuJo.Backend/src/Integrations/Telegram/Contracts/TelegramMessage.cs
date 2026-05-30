namespace BuJo.Integrations.Telegram.Contracts;

public sealed record TelegramMessage(
    long ChatId,
    long FromTelegramId,
    string? FromUsername,
    string? Text);