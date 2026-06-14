using BuJo.Contracts.V1.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

internal sealed class HabitService : IHabitService
{
    private const int NameMaxLength = 200;

    private readonly IHabitRepository _habitRepository;
    private readonly IHabitLogRepository _habitLogRepository;

    public HabitService(IHabitRepository habitRepository, IHabitLogRepository habitLogRepository)
    {
        _habitRepository = habitRepository;
        _habitLogRepository = habitLogRepository;
    }

    public async Task<HabitResponse> CreateAsync(CreateHabitCommand command, CancellationToken ct)
    {
        var name = command.Name.Trim();

        switch (name.Length)
        {
            case 0:
                throw new ArgumentException("Название привычки не может быть пустым", nameof(command));
            case > NameMaxLength:
                throw new ArgumentException($"Название привычки не может быть длиннее {NameMaxLength} символов",
                    nameof(command));
        }

        var duplicateExists = await _habitRepository.AnyBySpecAsync(
            new GetHabitsSpecification(new GetHabitsQuery(command.UserId, IncludeArchived: false, Name: name)),
            ct);

        if (duplicateExists)
            throw new InvalidOperationException($"Привычка с названием «{name}» уже существует");

        var habit = Habit.Create(command.UserId, name);
        await _habitRepository.AddAsync(habit, ct);

        return habit.ToResponse();
    }

    public async Task<IReadOnlyList<HabitResponse>> GetListAsync(GetHabitsQuery query, CancellationToken ct)
    {
        var habits = await _habitRepository.ListBySpecAsync(new GetHabitsSpecification(query), ct);

        return habits.Select(h => h.ToResponse()).ToList();
    }

    public async Task<Habit?> GetByIdAsync(Guid habitId, CancellationToken ct)
    {
        return await _habitRepository.GetByIdAsync(habitId, ct);
    }

    public async Task<HabitLogResponse> LogAsync(LogHabitCommand command, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (command.Date > today)
            throw new ArgumentException("Дата не может быть в будущем", nameof(command));

        var habit = await _habitRepository.GetByIdAsync(command.HabitId, ct);
        if (habit is null)
            throw new InvalidOperationException($"Привычка с ID {command.HabitId} не найдена");

        if (habit.UserId != command.UserId)
            throw new InvalidOperationException("Привычка не принадлежит текущему пользователю");

        var existing = await _habitLogRepository.FirstOrDefaultBySpecAsync(
            new GetHabitLogBySpecification(command.HabitId, specificDate: command.Date), ct);

        if (existing is not null)
        {
            existing.SetCompleted(command.IsCompleted);
            await _habitLogRepository.UpdateAsync(existing, ct);
            return existing.ToResponse(habit.Name);
        }

        var log = HabitLog.Create(command.HabitId, command.Date, command.IsCompleted);
        await _habitLogRepository.AddAsync(log, ct);

        return log.ToResponse(habit.Name);
    }

    public async Task<HabitStatsResponse> GetStatsAsync(GetHabitStatsQuery query, CancellationToken ct)
    {
        var habit = await _habitRepository.GetByIdAsync(query.HabitId, ct);
        
        if (habit is null)
            throw new InvalidOperationException($"Привычка с ID {query.HabitId} не найдена");

        if (habit.UserId != query.UserId)
            throw new InvalidOperationException("Привычка не принадлежит текущему пользователю");

        var (fromDate, toDate) = GetDateRange(query.Period);

        var logs = await _habitLogRepository.ListBySpecAsync(
            new GetHabitLogBySpecification(query.HabitId, fromDate: fromDate, toDate: toDate), ct);

        var completedLogs = logs.Where(l => l.IsCompleted).OrderBy(l => l.Date).ToList();

        var calendarDays = new List<CalendarDay>();
        var completedDates = logs.Where(l => l.IsCompleted).Select(l => l.Date).ToHashSet();
        for (var d = fromDate; d <= toDate; d = d.AddDays(1))
        {
            calendarDays.Add(CalendarDay.Create(d, completedDates.Contains(d)));
        }

        var monthlyStats = logs
            .GroupBy(l => new { l.Date.Year, l.Date.Month })
            .Select(g => MonthlyStats.Create(
                g.Key.Year,
                g.Key.Month,
                g.Count(l => l.IsCompleted),
                g.Count()))
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();
        
        var streak = 0;
        var bestStreak = 0;
        DateOnly? prevDate = null;

        foreach (var log in logs.OrderBy(l => l.Date))
        {
            if (!log.IsCompleted)
            {
                streak = 0;
                prevDate = null;
                continue;
            }

            if (prevDate is not null && log.Date == prevDate.Value.AddDays(1))
            {
                streak++;
            }
            else
            {
                streak = 1;
            }

            if (streak > bestStreak)
                bestStreak = streak;

            prevDate = log.Date;
        }

        var currentStreak = 0;
        if (completedLogs.Count > 0)
        {
            var lastCompleted = completedLogs.Last().Date;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (lastCompleted == today || lastCompleted == today.AddDays(-1))
            {
                currentStreak = streak;
            }
        }

        var totalInPeriod = logs.Count;
        var totalCompletedInPeriod = completedLogs.Count;
        var completionRate = totalInPeriod > 0
            ? Math.Round((double)totalCompletedInPeriod / totalInPeriod * 100, 1)
            : 0;

        return HabitStatsResponse.Create(
            currentStreak,
            bestStreak,
            completionRate,
            totalCompletedInPeriod,
            monthlyStats,
            calendarDays);
    }

    public async Task<IReadOnlyList<HabitLogResponse>> GetLogsAsync(GetHabitLogsQuery query, CancellationToken ct)
    {
        var logs = await _habitLogRepository.ListBySpecAsync(
            new GetHabitLogBySpecification(
                query.HabitId, 
                userId: query.UserId,
                fromDate: query.FromDate,
                toDate: query.ToDate), ct);

        return logs
            .Select(l => l.ToResponse(l.Habit.Name))
            .ToList();
    }

    private static (DateOnly From, DateOnly To) GetDateRange(StatsPeriod period)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return period switch
        {
            StatsPeriod.Week => (today.AddDays(-(int)today.DayOfWeek), today.AddDays(6 - (int)today.DayOfWeek)),

            StatsPeriod.Month => (new DateOnly(today.Year, today.Month, 1),
                new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month))),

            StatsPeriod.Quarter => (today.AddMonths(-3), today),

            StatsPeriod.All => (DateOnly.MinValue, today),

            _ => (new DateOnly(today.Year, today.Month, 1),
                new DateOnly(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month))),
        };
    }
}
