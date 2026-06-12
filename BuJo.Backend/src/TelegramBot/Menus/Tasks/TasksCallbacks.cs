using BuJo.TelegramBot.Menus.Main;

namespace BuJo.TelegramBot.Menus.Tasks;

/// <summary>
/// Константы callback_data для экранов управления задачами
/// </summary>
public static class TasksCallbacks
{
    public const string Prefix = "tasks";
    
    /// <summary>
    /// Создать новую задачу
    /// </summary>
    public const string Create = Prefix + ":create";
}