using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

internal sealed class UserBotStateService(IUserBotStateRepository repository) : IUserBotStateService
{
    public async Task<UserBotState> GetOrCreateAsync(Guid userId, CancellationToken ct = default)
    {
        var state = await repository.GetByUserIdAsync(userId, ct);
        if (state is not null)
            return state;

        state = UserBotState.Create(userId);
        await repository.AddAsync(state, ct);
        return state;
    }

    public async Task UpdateLastMenuMessageAsync(Guid userId, int messageId, CancellationToken ct = default)
    {
        var state = await GetExistingAsync(userId, ct);
        state.SetLastMenuMessage(messageId);
        await repository.UpdateAsync(state, ct);
    }

    public async Task SetPendingActionAsync(Guid userId, PendingAction action, string? payload, CancellationToken ct = default)
    {
        var state = await GetExistingAsync(userId, ct);
        state.SetPendingAction(action, payload);
        await repository.UpdateAsync(state, ct);
    }

    public async Task ClearPendingActionAsync(Guid userId, CancellationToken ct = default)
    {
        var state = await GetExistingAsync(userId, ct);
        state.ClearPendingAction();
        await repository.UpdateAsync(state, ct);
    }

    private async Task<UserBotState> GetExistingAsync(Guid userId, CancellationToken ct)
        => await repository.GetByUserIdAsync(userId, ct)
            ?? throw new InvalidOperationException($"Состояние диалога не найдено для UserId={userId}");
}
