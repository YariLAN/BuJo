using BuJo.Contracts.V1.Accounting;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

/// <summary>
/// Сервис для работы с сущностью <inheritdoc cref="User"/>
/// </summary>
public interface IUserService
{
    Task<UserResponse?> GetOrDefaultAsync(GetUserQuery query, CancellationToken ct = default);

    Task<UserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct = default);
}