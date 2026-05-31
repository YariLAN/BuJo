using Telegram.Bot.Types.ReplyMarkups;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Готовое к отправке состояние меню — текст и inline-клавиатура
/// </summary>
public sealed record MenuView(string Text, InlineKeyboardMarkup Markup);
