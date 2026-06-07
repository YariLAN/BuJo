using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus.Main;

/// <summary>
/// Сборщик экрана-заглушки для пунктов меню, ещё не реализованных
/// </summary>
public static class StubMenuBuilder
{
    public static MenuView Build(string title)
    {
        var text = $"🚧 {title}\nСкоро будет реализовано.";

        var markup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬅️ Назад", MenuCallbacks.Main),
            },
        });

        return new MenuView(text, markup);
    }
}
