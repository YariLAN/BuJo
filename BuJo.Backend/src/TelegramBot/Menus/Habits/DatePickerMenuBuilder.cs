using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Строит экран выбора даты для отметки привычки
/// </summary>
internal static class DatePickerMenuBuilder
{
    public static MenuView Build()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var dayBeforeYesterday = today.AddDays(-2);

        var text = "📅 Выберите дату:";

        var keyboard = new List<List<InlineKeyboardButton>>
        {
            ([InlineKeyboardButton.WithCallbackData($"Сегодня ({today:dd.MM})", HabitCallbacks.MarkToday)]),
            ([InlineKeyboardButton.WithCallbackData($"Вчера ({yesterday:dd.MM})", HabitCallbacks.MarkYesterday)]),
            ([
                InlineKeyboardButton.WithCallbackData($"Позавчера ({dayBeforeYesterday:dd.MM})",
                    HabitCallbacks.MarkDayBeforeYesterday)
            ]),
            ([InlineKeyboardButton.WithCallbackData("Своя дата (введу вручную)", HabitCallbacks.MarkCustomDate)]),
            ([InlineKeyboardButton.WithCallbackData("🏠 Назад", HabitCallbacks.BackToList)]),
        };

        return new MenuView(text, new InlineKeyboardMarkup(keyboard));
    }
}
