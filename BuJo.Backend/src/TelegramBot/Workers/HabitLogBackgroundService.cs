using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using BuJo.Contracts.V1.Habits;
using BuJo.Domain.Accounting;
using BuJo.TelegramBot.Menus.Habits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BuJo.TelegramBot.Workers;

/// <summary>
/// Фоновый сервис для отправки вечерних напоминаний об отметке привычек.
/// Каждую минуту проверяет пользователей, у которых установлено ReminderEveningTime,
/// совпадающее с текущим временем, и отправляет чек-лист с кнопками для отметки.
/// </summary>
internal sealed class HabitLogBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<HabitLogBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HabitLogBackgroundService запущен");

        using var timer = new PeriodicTimer(CheckInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await SendEveningRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при отправке вечерних напоминаний");
            }
        }
    }

    private async Task SendEveningRemindersAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var habitService = scope.ServiceProvider.GetRequiredService<IHabitService>();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        var now = DateTimeOffset.UtcNow;
        var currentTime = TimeOnly.FromDateTime(now.DateTime);

        logger.LogDebug("Проверка пользователей для времени {Time}", currentTime);

        // Get all users — we need to find those whose ReminderEveningTime matches
        // This is a simplified approach; for production, a DB query would be more efficient
        // For now we use the existing query infrastructure
        var allUsers = await GetAllUsersWithEveningReminderAsync(userRepository, ct);

        foreach (var user in allUsers)
        {
            if (user.ReminderEveningTime is null)
                continue;

            // Check if current minute matches reminder time
            if (user.ReminderEveningTime.Value.Hour != currentTime.Hour ||
                user.ReminderEveningTime.Value.Minute != currentTime.Minute)
                continue;

            // Check if user has any habits
            var habits = await habitService.GetListAsync(new GetHabitsQuery(user.Id), ct);
            if (habits.Count == 0)
                continue;

            // Send the checklist
            await SendChecklistAsync(botClient, user, habits, ct);
        }
    }

    private static async Task<IReadOnlyList<User>> GetAllUsersWithEveningReminderAsync(
        IUserRepository userRepository, CancellationToken ct)
    {
        // Use the specification to get users with evening reminder set
        var spec = new UsersWithEveningReminderSpec();
        var users = await userRepository.ListAsync(spec, ct);
        return users;
    }

    private async Task SendChecklistAsync(
        ITelegramBotClient botClient,
        User user,
        IReadOnlyList<HabitResponse> habits,
        CancellationToken ct)
    {
        var chatId = long.Parse(user.TelegramId);

        var lines = new List<string>
        {
            "🌆 *Вечерний чек-лист привычек!*",
            string.Empty,
            "Отметьте, что сделали сегодня:",
            string.Empty,
        };

        var keyboard = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();

        for (int i = 0; i < habits.Count; i++)
        {
            var habit = habits[i];
            lines.Add($"{i + 1}. {habit.Name}");

            keyboard.Add([
                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                    $"✅ {habit.Name}",
                    $"{HabitCallbacks.Prefix}:select_{habit.Id}"),
            ]);
        }

        keyboard.Add([
            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                "✅ Всё отметил(а)", "habits:checklist_done"),
        ]);

        try
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: string.Join("\n", lines),
                replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(keyboard),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось отправить напоминание пользователю {UserId}", user.Id);
        }
    }
}