using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.TelegramBot.Menus.Main;
using BuJo.TelegramBot.Services;
using BuJo.TelegramBot.Services.Main;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class MenuCallbackHandler(
    IUserService userService,
    ITelegramBotClient botClient,
    IMenuService menuService,
    ILogger<MenuCallbackHandler> logger) : CallbackHandlerBase(botClient, userService, logger)
{
    public override string Prefix => MenuCallbacks.Prefix;

    protected override async Task HandleCallbackAsync(Guid userId, CallbackQuery callbackQuery, CancellationToken ct = default)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data;
        
        switch (data)
        {
            case MenuCallbacks.Main:
                await menuService.OpenMainAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.Settings:
                await menuService.OpenSettingsAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.HabitsList:
                await menuService.OpenStubAsync(userId, chatId, "Привычки", ct);
                return;

            case MenuCallbacks.HabitCreate:
                await menuService.OpenStubAsync(userId, chatId, "Создание привычки", ct);
                return;

            case MenuCallbacks.TasksList:
                await menuService.OpenTasksAsync(userId, chatId, ct);
                return;

            case MenuCallbacks.TaskCreate:
                await menuService.OpenTaskCreateAsync(userId, chatId, ct);
                return;
        }
        
        logger.LogWarning("Unknown menu callback data: {Data}", data);
    }
}
