using Ardalis.Specification;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

/// <summary>
/// Репозиторий состояний диалога пользователя с ботом
/// </summary>
public interface IUserBotStateRepository : IRepositoryBase<UserBotState>
{
    /// <summary>
    /// Получить состояние диалога по идентификатору пользователя
    /// </summary>
    Task<UserBotState?> GetByUserIdAsync(Guid userId, long chatId, CancellationToken ct);
}
