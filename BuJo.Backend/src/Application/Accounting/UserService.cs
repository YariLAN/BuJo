using BuJo.Contracts.V1.Accounting;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

internal sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponse?> GetOrDefaultAsync(GetUserQuery query, CancellationToken ct = default)
    {
        var user = await userRepository.GetBySpecAsync(new GetUserSpecification(query), ct);

        return user?.ToResponse();
    }

    public async Task<UserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        var existingUser = await userRepository.GetBySpecAsync(new GetUserSpecification(
                new GetUserQuery(null, TelegramId: command.TelegramId)),
            ct);

        if (existingUser is not null)
            throw new InvalidOperationException($"Пользователь с TelegramId {command.TelegramId} уже существует");

        var user = User.Create(command.TelegramId, command.Username);
        await userRepository.AddAsync(user, ct);

        return user.ToResponse();
    }
}