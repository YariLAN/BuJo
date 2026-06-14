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

    /// <summary>
    /// Открыть подменю конкретной привычки (отметить/статистика/назад)
    /// </summary>
    Task OpenHabitMenuAsync(Guid userId, long chatId, Guid habitId, CancellationToken ct);

    /// <summary>
    /// Открыть выбор даты для отметки привычки
    /// </summary>
    Task OpenDatePickerAsync(Guid userId, long chatId, Guid habitId, CancellationToken ct);

    /// <summary>
    /// Отметить привычку за указанную дату
    /// </summary>
    Task MarkHabitAsync(Guid userId, long chatId, Guid habitId, DateOnly date, bool isCompleted, CancellationToken ct);

    /// <summary>
    /// Показать результат отметки
    /// </summary>
    Task ShowMarkResultAsync(Guid userId, long chatId, string habitName, DateOnly date, bool isCompleted, CancellationToken ct);

    /// <summary>
    /// Открыть экран выбора привычки для статистики
    /// </summary>
    Task OpenStatsSelectionAsync(Guid userId, long chatId, CancellationToken ct);

    /// <summary>
    /// Показать статистику для конкретной привычки
    /// </summary>
    Task OpenStatsForHabitAsync(Guid userId, long chatId, Guid habitId, CancellationToken ct);
}
