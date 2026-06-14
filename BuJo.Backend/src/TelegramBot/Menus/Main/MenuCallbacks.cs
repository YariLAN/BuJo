namespace BuJo.TelegramBot.Menus.Main;

/// <summary>
/// Константы и фабрики значений callback_data для inline-кнопок меню
/// </summary>
public static class MenuCallbacks
{
    /// <summary>
    /// Префикс домена меню — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "menu";

    public const string Main = Prefix + ":main";

    public const string Settings = Prefix + ":settings";

    public const string HabitsList = Prefix + ":habits";

    public const string HabitCreate = Prefix + ":habit:create";

    public const string TasksList = Prefix + ":tasks";

    public const string TaskCreate = Prefix + ":task:create";
}
