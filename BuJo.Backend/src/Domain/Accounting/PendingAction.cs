namespace BuJo.Domain.Accounting;

/// <summary>
/// Тип ожидаемого ввода от пользователя
/// </summary>
public enum PendingAction
{
    /// <summary>
    /// Ничего не ожидается
    /// </summary>
    None = 0,

    /// <summary>
    /// Ожидается ввод времени утреннего напоминания в формате HH:MM
    /// </summary>
    AwaitingMorningTime,

    /// <summary>
    /// Ожидается ввод времени вечернего напоминания в формате HH:MM
    /// </summary>
    AwaitingEveningTime,

    /// <summary>
    /// Ожидается ввод названия новой привычки
    /// </summary>
    AwaitingHabitName,

    /// <summary>
    /// Ожидается выбор привычки из нумерованного списка (для отметки выполнения)
    /// </summary>
    AwaitingHabitSelect,

    /// <summary>
    /// Ожидается ввод даты для отметки привычки вручную
    /// </summary>
    AwaitingHabitLogDate,

    /// <summary>
    /// Ожидается выбор привычки из нумерованного списка (для статистики)
    /// </summary>
    AwaitingStatsHabitSelect,
}
