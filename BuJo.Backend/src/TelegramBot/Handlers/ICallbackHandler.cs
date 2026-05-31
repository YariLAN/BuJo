using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers;

/// <summary>
/// Обработчик CallbackQuery определённого префикса callback_data
/// </summary>
public interface ICallbackHandler
{
    /// <summary>
    /// Префикс callback_data — UpdateDispatcher по нему маршрутизирует CallbackQuery
    /// </summary>
    string Prefix { get; }

    Task HandleAsync(CallbackQuery callback, CancellationToken ct);
}
