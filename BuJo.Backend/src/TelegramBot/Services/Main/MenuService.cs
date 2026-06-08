using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.TelegramBot.Menus.Main;
using BuJo.TelegramBot.Menus.Settings;

namespace BuJo.TelegramBot.Services.Main;

internal sealed class MenuService(
    MenuRenderer renderer,
    IUserBotStateService userBotStateService) : IMenuService
{
    public async Task StartAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = MainMenuBuilder.Build();
        await renderer.RecreateAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenMainAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = MainMenuBuilder.Build();
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenSettingsAsync(Guid userId, long chatId, CancellationToken ct)
    {
        var view = SettingsMenuBuilder.Build();
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }

    public async Task OpenStubAsync(Guid userId, long chatId, string title, CancellationToken ct)
    {
        var view = StubMenuBuilder.Build(title);
        await renderer.EditAsync(userId, chatId, view, ct);
        await userBotStateService.ClearPendingActionAsync(userId, chatId, ct);
    }
}
