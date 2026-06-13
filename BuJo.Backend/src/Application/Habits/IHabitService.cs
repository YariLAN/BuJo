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
}
