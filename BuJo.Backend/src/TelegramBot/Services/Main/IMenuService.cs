namespace BuJo.TelegramBot.Services.Main;

/// <summary>
/// Сервис навигации по главным экранам бота (главное меню, настройки-хаб, заглушки)
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// /start: удалить старое сообщение-меню и отправить новое главное меню
    /// </summary>
    Task StartAsync(Guid userId, long chatId, CancellationToken ct = default);

    /// <summary>
    /// Открыть главное меню (редактирует активное сообщение-меню)
    /// </summary>
    Task OpenMainAsync(Guid userId, long chatId, CancellationToken ct = default);

    /// <summary>
    /// Открыть хаб настроек
    /// </summary>
    Task OpenSettingsAsync(Guid userId, long chatId, CancellationToken ct = default);

    /// <summary>
    /// Открыть экран-заглушку «скоро будет» с указанным заголовком
    /// </summary>
    Task OpenStubAsync(Guid userId, long chatId, string title, CancellationToken ct = default);
}
