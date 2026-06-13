namespace BuJo.Domain.Accounting;

/// <summary>
/// Состояние диалога пользователя с ботом
/// </summary>
public sealed class UserBotState
{
    private UserBotState(Guid id, Guid userId, long chatId, DateTimeOffset updatedAt)
    {
        Id = id;
        UserId = userId;
        ChatId = chatId;
        UpdatedAt = updatedAt;
    }

    public UserBotState() { }

    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Идентификатор чата Telegram
    /// </summary>
    public long ChatId { get; private set; }
    
    /// <summary>
    /// Идентификатор последнего сообщения с меню в чате с пользователем
    /// </summary>
    public int? LastMenuMessageId { get; private set; }

    /// <summary>
    /// Текущее ожидаемое действие пользователя
    /// </summary>
    public PendingAction PendingAction { get; private set; }

    /// <summary>
    /// JSON-контекст для текущего ожидаемого действия (накопленные слоты, идентификаторы редактируемых сущностей и т.п.)
    /// </summary>
    public string? PendingPayload { get; private set; }

    /// <summary>
    /// Дата последнего обновления записи
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Создать пустое состояние диалога для пользователя
    /// </summary>
    public static UserBotState Create(Guid userId, long chatId)
        => new(Guid.NewGuid(), userId, chatId, DateTimeOffset.UtcNow);

    /// <summary>
    /// Запомнить идентификатор сообщения с активным меню
    /// </summary>
    public void SetLastMenuMessage(int messageId)
    {
        LastMenuMessageId = messageId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Установить ожидаемое действие пользователя и (опционально) сопутствующий JSON-контекст
    /// </summary>
    public void SetPendingAction(PendingAction action, string? payload = null)
    {
        PendingAction = action;
        PendingPayload = payload;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Сбросить ожидаемое действие и связанный с ним контекст
    /// </summary>
    public void ClearPendingAction()
    {
        PendingAction = PendingAction.None;
        PendingPayload = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

/// <summary>
/// Тип ожидаемого ввода от пользователя
/// </summary>
public enum PendingAction
{
    /// <summary>
    /// Ничего не ожидается
    /// </summary>
    None = 0,

    /// <summary>
    /// Ожидается ввод времени утреннего напоминания в формате HH:MM
    /// </summary>
    AwaitingMorningTime,

    /// <summary>
    /// Ожидается ввод времени вечернего напоминания в формате HH:MM
    /// </summary>
    AwaitingEveningTime,
    
    /// <summary>
    /// Ожидается ввод названия задачи
    /// </summary>
    AwaitingTaskTitle,
    
    /// <summary>
    /// Ожидается ввод описания задачи
    /// </summary>
    AwaitingTaskDescription,
    
    /// <summary>
    /// Ожидается выбор приоритета задачи
    /// </summary>
    AwaitingTaskPriority,
    
    /// <summary>
    /// Ожидается ввод дедлайна задачи
    /// </summary>
    AwaitingTaskDueDate,

    /// <summary>
    /// Ожидается ввод названия новой привычки
    /// </summary>
    AwaitingHabitName,
}
