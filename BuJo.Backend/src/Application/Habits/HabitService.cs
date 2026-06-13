using BuJo.Contracts.V1.Habits;
using BuJo.Domain.Habits;

namespace BuJo.Application.Habits;

internal sealed class HabitService : IHabitService
{
    private const int NameMaxLength = 200;

    private readonly IHabitRepository _habitRepository;

    public HabitService(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task<HabitResponse> CreateAsync(CreateHabitCommand command, CancellationToken ct)
    {
        var name = (command.Name ?? string.Empty).Trim();

        if (name.Length == 0)
            throw new ArgumentException("Название привычки не может быть пустым", nameof(command));

        if (name.Length > NameMaxLength)
            throw new ArgumentException($"Название привычки не может быть длиннее {NameMaxLength} символов", nameof(command));

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
}
