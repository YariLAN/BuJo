namespace BuJo.TelegramBot.Services.Habits;

/// <summary>
/// Сервис навигации по разделу привычек в Telegram-боте
/// </summary>
public interface IHabitsMenuService
{
    /// <summary>
    /// Открыть список привычек пользователя
    /// </summary>
    Task OpenListAsync(Guid userId, long chatId, CancellationToken ct);

    /// <summary>
    /// Открыть экран ввода названия новой привычки и перевести бота в ожидание ввода
    /// </summary>
    Task OpenCreatePromptAsync(Guid userId, long chatId, CancellationToken ct);

    /// <summary>
    /// Показать экран успешного создания привычки
    /// </summary>
    Task ShowCreatedAsync(Guid userId, long chatId, string habitName, CancellationToken ct);

    /// <summary>
    /// Показать ошибку валидации названия, не сбрасывая ожидание ввода
    /// </summary>
    Task ShowValidationErrorAsync(Guid userId, long chatId, string error, CancellationToken ct);
}
