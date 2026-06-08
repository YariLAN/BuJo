using BuJo.Application.Accounting.Abstractions;
using BuJo.Contracts.V1.Accounting;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserResponse?> GetOrDefaultAsync(GetUserQuery query, CancellationToken ct)
    {
        var user = await _userRepository.GetBySpecAsync(new GetUserSpecification(query), ct);

        return user?.ToResponse();
    }

    public async Task<UserResponse> CreateAsync(CreateUserCommand command, CancellationToken ct)
    {
        var existingUser = await _userRepository.GetBySpecAsync(new GetUserSpecification(
                new GetUserQuery(null, TelegramId: command.TelegramId)),
            ct);

        if (existingUser is not null)
            throw new InvalidOperationException($"Пользователь с TelegramId {command.TelegramId} уже существует");

        var user = User.Create(command.TelegramId, command.Username);
        await _userRepository.AddAsync(user, ct);

        return user.ToResponse();
    }

    public async Task SetReminderAsync(Guid userId, ReminderKind kind, TimeOnly time, CancellationToken ct)
    {
        var user = await _userRepository.GetBySpecAsync(new GetUserSpecification(new GetUserQuery(userId)), ct)
            ?? throw new InvalidOperationException($"Пользователь с Id={userId} не найден");

        user.SetReminder(kind, time);
        await _userRepository.UpdateAsync(user, ct);
    }
}