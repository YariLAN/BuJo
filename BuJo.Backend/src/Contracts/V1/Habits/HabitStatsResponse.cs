namespace BuJo.Contracts.V1.Habits;

public sealed class HabitStatsResponse
{
    /// <summary>
    /// Текущая серия (streak) — сколько дней подряд отмечено
    /// </summary>
    public int CurrentStreak { get; init; }
    
    /// <summary>
    /// Максимальная серия за всё время
    /// </summary>
    public int BestStreak { get; init; }
    
    /// <summary>
    /// Процент выполнения за выбранный период (0–100)
    /// </summary>
    public double CompletionRate { get; init; }

    /// <summary>
    /// Общее количество отметок за период
    /// </summary>
    public int TotalCompleted { get; init; }

    /// <summary>
    /// Статистика по месяцам
    /// </summary>
    public IReadOnlyList<MonthlyStats> MonthlyStats { get; init; } = [];

    /// <summary>
    /// Календарь отметок (для отображения сетки)
    /// </summary>
    public IReadOnlyList<CalendarDay> CalendarDays { get; init; } = [];

    public static HabitStatsResponse Create(
        int currentStreak,
        int bestStreak,
        double completionRate,
        int totalCompleted,
        IReadOnlyList<MonthlyStats> monthlyStats,
        IReadOnlyList<CalendarDay> calendarDays) => new()
    {
        CurrentStreak = currentStreak,
        BestStreak = bestStreak,
        CompletionRate = completionRate,
        TotalCompleted = totalCompleted,
        MonthlyStats = monthlyStats,
        CalendarDays = calendarDays,
    };
}

public sealed class MonthlyStats
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int CompletedDays { get; init; }
    public int TotalDays { get; init; }

    public static MonthlyStats Create(int year, int month, int completedDays, int totalDays) => new()
    {
        Year = year,
        Month = month,
        CompletedDays = completedDays,
        TotalDays = totalDays,
    };
}

public sealed class CalendarDay
{
    public DateOnly Date { get; init; }
    public bool IsCompleted { get; init; }

    public static CalendarDay Create(DateOnly date, bool isCompleted) => new()
    {
        Date = date,
        IsCompleted = isCompleted,
    };
}