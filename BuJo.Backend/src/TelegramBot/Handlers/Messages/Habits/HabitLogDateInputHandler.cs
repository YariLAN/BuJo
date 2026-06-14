using BuJo.Application.Accounting;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Services.Habits;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Messages.Habits;

/// <summary>
/// Обрабатывает ввод даты для отметки привычки вручную
/// </summary>
public sealed class HabitLogDateInputHandler(
    IUserBotStateService userBotStateService,
    IHabitsMenuService habitsMenuService,
    ILogger<HabitLogDateInputHandler> logger) : IPendingInputHandler
{
    public bool CanHandle(PendingAction action) => action == PendingAction.AwaitingHabitLogDate;

    public async Task HandleAsync(Guid userId, long chatId, PendingAction action, Message message, CancellationToken ct)
    {
        if (message.Text is null)
            return;

        var text = message.Text.Trim();

        if (!DateOnly.TryParse(text, out var date))
        {
            if (!DateOnly.TryParseExact(text, "dd.MM.yyyy", out date) &&
                !DateOnly.TryParseExact(text, "d.M.yyyy", out date))
            {
                // Не сбрасываем PendingAction при некорректном формате
                await habitsMenuService.ShowValidationErrorAsync(userId, chatId,
                    "Неверный формат даты. Пожалуйста, введите дату в формате ДД.ММ.ГГГГ (например, 01.06.2026)", ct);
                return;
            }
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date > today)
        {
            await habitsMenuService.ShowValidationErrorAsync(userId, chatId,
                "Дата не может быть в будущем", ct);
            return;
        }

        // Read habitId from the pending payload (stored as raw GUID string)
        var state = await userBotStateService.GetOrCreateAsync(userId, chatId, ct);
        var habitId = ParseHabitId(state.PendingPayload);

        if (habitId is null)
        {
            logger.LogWarning("No habitId in payload for HabitLogDateInputHandler");
            await habitsMenuService.ShowValidationErrorAsync(userId, chatId,
                "Ошибка: не удалось определить привычку. Пожалуйста, вернитесь в меню и попробуйте снова.", ct);
            return;
        }

        await habitsMenuService.MarkHabitAsync(userId, chatId, habitId.Value, date, true, ct: ct);
    }

    private static Guid? ParseHabitId(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return null;

        return Guid.TryParse(payload, out var habitId) ? habitId : null;
    }
}