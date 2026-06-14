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

    /// <summary>
    /// Отметить выполнение за сегодня
    /// </summary>
    public const string MarkToday = Prefix + ":mark_today";

    /// <summary>
    /// Выбрать другую дату для отметки
    /// </summary>
    public const string MarkOtherDate = Prefix + ":mark_other_date";

    /// <summary>
    /// Пропустить (отметить невыполненным)
    /// </summary>
    public const string MarkSkip = Prefix + ":mark_skip";

    /// <summary>
    /// Вчера
    /// </summary>
    public const string MarkYesterday = Prefix + ":mark_yesterday";

    /// <summary>
    /// Позавчера
    /// </summary>
    public const string MarkDayBeforeYesterday = Prefix + ":day_before_yesterday";

    /// <summary>
    /// Своя дата
    /// </summary>
    public const string MarkCustomDate = Prefix + ":custom_date";

    /// <summary>
    /// Посмотреть статистику
    /// </summary>
    public const string ViewStats = Prefix + ":stats";

    /// <summary>
    /// Выбрать привычку для статистики
    /// </summary>
    public const string StatsSelect = Prefix + ":stats_select";

    /// <summary>
    /// Назад к списку привычек
    /// </summary>
    public const string BackToList = Prefix + ":back_to_list";

    /// <summary>
    /// Переключить отметку привычки в чек-листе (вечернее напоминание)
    /// </summary>
    public const string ToggleHabit = Prefix + ":toggle_";

    /// <summary>
    /// Подтвердить чек-лист
    /// </summary>
    public const string ConfirmChecklist = Prefix + ":checklist_done";
}
