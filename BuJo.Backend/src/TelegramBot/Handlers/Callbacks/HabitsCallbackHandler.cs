using BuJo.Application.Accounting;
using BuJo.TelegramBot.Menus.Habits;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class HabitsCallbackHandler : CallbackHandlerBase
{
    private readonly IHabitsMenuService _habitsMenuService;
    private readonly ILogger<HabitsCallbackHandler> _logger;

    public HabitsCallbackHandler(
        ITelegramBotClient botClient,
        IUserService userService,
        IHabitsMenuService habitsMenuService,
        ILogger<HabitsCallbackHandler> logger) : base(botClient, userService, logger)
    {
        _habitsMenuService = habitsMenuService;
        _logger = logger;
    }

    public override string Prefix => HabitCallbacks.Prefix;

    protected override async Task HandleCallbackAsync(Guid userId, CallbackQuery callbackQuery, CancellationToken ct = default)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data!;

        switch (data)
        {
            case HabitCallbacks.Add:
                await _habitsMenuService.OpenCreatePromptAsync(userId, chatId, ct);
                return;
        }

        _logger.LogWarning("Unknown habits callback data: {Data}", data);
    }
}
