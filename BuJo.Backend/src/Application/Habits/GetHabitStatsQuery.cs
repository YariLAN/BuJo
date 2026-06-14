namespace BuJo.Application.Habits;

/// <summary>Период для статистики</summary>
public enum StatsPeriod
{
    /// <summary>Текущая неделя</summary>
    Week,
    
    /// <summary>Текущий месяц</summary>
    Month,
    
    /// <summary>Последние 3 месяца</summary>
    Quarter,
    
    /// <summary>Всё время</summary>
    All,
}

public sealed record GetHabitStatsQuery(Guid UserId, Guid HabitId, StatsPeriod Period = StatsPeriod.Month);