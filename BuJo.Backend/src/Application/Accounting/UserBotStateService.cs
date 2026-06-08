using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

internal sealed class UserBotStateService(IUserBotStateRepository repository) : IUserBotStateService
{
    public async Task<UserBotState> GetOrCreateAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var state = await repository.GetByUserIdAsync(userId, chatId, ct);
        if (state is not null)
            return state;

        state = UserBotState.Create(userId, chatId);
        await repository.AddAsync(state, ct);
        return state;
    }

    public async Task UpdateLastMenuMessageAsync(Guid userId, long chatId, int messageId, CancellationToken ct)
    {
        var state = await GetExistingAsync(userId, chatId, ct);
        state.SetLastMenuMessage(messageId);
        await repository.UpdateAsync(state, ct);
    }

    public async Task SetPendingActionAsync(Guid userId, long chatId, PendingAction action, string? payload, CancellationToken ct)
    {
        var state = await GetExistingAsync(userId, chatId, ct);
        state.SetPendingAction(action, payload);
        await repository.UpdateAsync(state, ct);
    }

    public async Task ClearPendingActionAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var state = await GetExistingAsync(userId, chatId, ct);
        state.ClearPendingAction();
        await repository.UpdateAsync(state, ct);
    }

    private async Task<UserBotState> GetExistingAsync(Guid userId, long chatId, CancellationToken ct)
        => await repository.GetByUserIdAsync(userId, chatId, ct)
            ?? throw new InvalidOperationException($"Состояние диалога не найдено для UserId={userId}");
}
