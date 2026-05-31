using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

/// <summary>
/// Сервис для работы с состоянием диалога пользователя с ботом
/// </summary>
public interface IUserBotStateService
{
    /// <summary>
    /// Получить состояние диалога или создать пустое, если его ещё нет
    /// </summary>
    Task<UserBotState> GetOrCreateAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Запомнить идентификатор сообщения с активным меню
    /// </summary>
    Task UpdateLastMenuMessageAsync(Guid userId, int messageId, CancellationToken ct = default);

    /// <summary>
    /// Установить ожидаемое действие пользователя и сопутствующий JSON-контекст
    /// </summary>
    Task SetPendingActionAsync(Guid userId, PendingAction action, string? payload, CancellationToken ct = default);

    /// <summary>
    /// Сбросить ожидаемое действие и связанный с ним контекст
    /// </summary>
    Task ClearPendingActionAsync(Guid userId, CancellationToken ct = default);
}
