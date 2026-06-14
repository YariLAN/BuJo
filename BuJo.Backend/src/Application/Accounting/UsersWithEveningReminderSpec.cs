using Ardalis.Specification;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

/// <summary>
/// Выбрать пользователей, у которых установлено время вечернего напоминания
/// </summary>
public sealed class UsersWithEveningReminderSpec : Specification<User>
{
    public UsersWithEveningReminderSpec()
    {
        Query
            .Where(u => u.ReminderEveningTime != null)
            .Include(u => u.Habits);
    }
}