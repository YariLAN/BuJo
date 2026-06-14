using BuJo.Contracts.V1.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

/// <summary>
/// Сервис для работы с сущностью <inheritdoc cref="Habit"/>
/// </summary>
public interface IHabitService
{
    /// <summary>
    /// Создать новую привычку
    /// </summary>
    Task<HabitResponse> CreateAsync(CreateHabitCommand command, CancellationToken ct = default);

    /// <summary>
    /// Получить список привычек пользователя
    /// </summary>
    Task<IReadOnlyList<HabitResponse>> GetListAsync(GetHabitsQuery query, CancellationToken ct = default);

    /// <summary>
    /// Получить привычку по ID (для проверки владельца)
    /// </summary>
    Task<Habit?> GetByIdAsync(Guid habitId, CancellationToken ct = default);

    /// <summary>
    /// Отметить выполнение привычки (upsert)
    /// </summary>
    Task<HabitLogResponse> LogAsync(LogHabitCommand command, CancellationToken ct = default);

    /// <summary>
    /// Получить статистику привычки
    /// </summary>
    Task<HabitStatsResponse> GetStatsAsync(GetHabitStatsQuery query, CancellationToken ct = default);

    /// <summary>
    /// Получить логи привычек
    /// </summary>
    Task<IReadOnlyList<HabitLogResponse>> GetLogsAsync(GetHabitLogsQuery query, CancellationToken ct = default);
}
