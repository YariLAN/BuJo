using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus.Habits;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

public sealed class HabitsCallbackHandler : CallbackHandlerBase
{
    private readonly IHabitsMenuService _habitsMenuService;
    private readonly IUserBotStateService _userBotStateService;
    private readonly ILogger<HabitsCallbackHandler> _logger;

    public HabitsCallbackHandler(
        ITelegramBotClient botClient,
        IUserService userService,
        IHabitsMenuService habitsMenuService,
        IUserBotStateService userBotStateService,
        ILogger<HabitsCallbackHandler> logger) : base(botClient, userService, logger)
    {
        _habitsMenuService = habitsMenuService;
        _userBotStateService = userBotStateService;
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

            case HabitCallbacks.StatsSelect:
                await _habitsMenuService.OpenStatsSelectionAsync(userId, chatId, ct);
                return;

            case HabitCallbacks.BackToList:
                await _habitsMenuService.OpenListAsync(userId, chatId, ct);
                return;
        }
        
        var statsPrefix = HabitCallbacks.Prefix + ":stats_";
        if (data.StartsWith(statsPrefix, StringComparison.Ordinal))
        {
            var habitIdStr = data[statsPrefix.Length..];
            
            if (Guid.TryParse(habitIdStr, out var habitId))
            {
                await _habitsMenuService.OpenStatsForHabitAsync(userId, chatId, habitId, ct);
                return;
            }
            
            _logger.LogWarning("Invalid habitId in stats callback: {Data}", data);
            return;
        }
        
        var state = await _userBotStateService.GetOrCreateAsync(userId, chatId, ct);
        
        var habitIdFromPayload = ExtractHabitIdFromPayload(state.PendingPayload);

        if (habitIdFromPayload is null)
        {
            _logger.LogWarning("No habitId in payload for callback: {Data}", data);
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        switch (data)
        {
            case HabitCallbacks.MarkToday:
                await _habitsMenuService.MarkHabitAsync(userId, chatId, habitIdFromPayload.Value, today, true, ct);
                return;

            case HabitCallbacks.MarkSkip:
                await _habitsMenuService.MarkHabitAsync(userId, chatId, habitIdFromPayload.Value, today, false, ct);
                return;

            case HabitCallbacks.MarkYesterday:
                await _habitsMenuService.MarkHabitAsync(userId, chatId, habitIdFromPayload.Value, today.AddDays(-1), true, ct);
                return;

            case HabitCallbacks.MarkDayBeforeYesterday:
                await _habitsMenuService.MarkHabitAsync(userId, chatId, habitIdFromPayload.Value, today.AddDays(-2), true, ct);
                return;

            case HabitCallbacks.MarkOtherDate:
                await _habitsMenuService.OpenDatePickerAsync(userId, chatId, habitIdFromPayload.Value, ct);
                return;

            case HabitCallbacks.MarkCustomDate:
                await _userBotStateService.SetPendingActionAsync(userId, chatId,
                    PendingAction.AwaitingHabitLogDate,
                    payload: habitIdFromPayload.Value.ToString(), ct);
                
                await _habitsMenuService.OpenDatePickerAsync(userId, chatId, habitIdFromPayload.Value, ct);
                return;

            case HabitCallbacks.ViewStats:
                await _habitsMenuService.OpenStatsForHabitAsync(userId, chatId, habitIdFromPayload.Value, ct);
                return;
        }

        _logger.LogWarning("Unknown habits callback data: {Data}", data);
    }

    private static Guid? ExtractHabitIdFromPayload(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return null;

        return Guid.TryParse(payload, out var habitId) ? habitId : null;
    }
}
