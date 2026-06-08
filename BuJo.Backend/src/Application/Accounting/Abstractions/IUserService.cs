using BuJo.Contracts.V1.Accounting;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting.Abstractions;

/// <summary>
/// Сервис для работы с сущностью <inheritdoc cref="User"/>
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Найти пользователя по запросу
    /// </summary>
    Task<UserResponse?> GetOrDefaultAsync(GetUserQuery query, CancellationToken ct = default);

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    Task<UserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct = default);

    /// <summary>
    /// Установить время утреннего или вечернего напоминания пользователя
    /// </summary>
    Task SetReminderAsync(Guid userId, ReminderKind kind, TimeOnly time, CancellationToken ct = default);
}