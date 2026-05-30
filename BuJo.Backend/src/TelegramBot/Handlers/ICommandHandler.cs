using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers;

/// <summary>
/// Команда
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Наименование команды
    /// </summary>
    string Command { get; }
    
    Task HandleAsync(Message message, CancellationToken ct);
}