namespace BuJo.TelegramBot.Menus.Habits;

/// <summary>
/// Константы callback_data для inline-кнопок раздела привычек
/// </summary>
public static class HabitCallbacks
{
    /// <summary>
    /// Префикс домена привычек — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "habits";

    /// <summary>
    /// Кнопка «➕ Добавить» внутри экрана списка привычек
    /// </summary>
    public const string Add = Prefix + ":add";
}
