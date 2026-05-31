using BuJo.Domain.Accounting;

namespace BuJo.TelegramBot.Services;

/// <summary>
/// Сервис навигации по разделу «Настройки»
/// </summary>
public interface ISettingsMenuService
{
    /// <summary>
    /// Открыть экран настроек напоминаний — текущие времена + кнопки выбора утра/вечера
    /// </summary>
    Task OpenRemindersAsync(Guid userId, long chatId, CancellationToken ct = default);

    /// <summary>
    /// Открыть экран выбора времени напоминания (пресеты + «Свой вариант»)
    /// </summary>
    Task OpenTimePickerAsync(Guid userId, long chatId, ReminderKind kind, CancellationToken ct = default);

    /// <summary>
    /// Открыть экран ручного ввода времени напоминания и установить ожидание текстового ввода.
    /// При <paramref name="withError"/> = true показывается с подсказкой о неверном предыдущем вводе.
    /// </summary>
    Task OpenCustomTimePromptAsync(Guid userId, long chatId, ReminderKind kind, bool withError = false, CancellationToken ct = default);
}
