using BuJo.Domain.Accounting;

namespace BuJo.TelegramBot.Menus;

/// <summary>
/// Константы и фабрики значений callback_data для inline-кнопок меню
/// </summary>
public static class MenuCallbacks
{
    /// <summary>
    /// Префикс домена меню — используется UpdateDispatcher для маршрутизации CallbackQuery
    /// </summary>
    public const string Prefix = "menu";

    public const string Main = "menu:main";

    public const string Settings = "menu:settings";

    public const string HabitsList = "menu:habits";

    public const string HabitCreate = "menu:habit:create";

    public const string TasksList = "menu:tasks";

    public const string TaskCreate = "menu:task:create";
}
