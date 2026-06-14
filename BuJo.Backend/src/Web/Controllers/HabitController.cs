using BuJo.Application.Accounting;
using BuJo.Application.Habits;
using BuJo.Contracts.V1;
using BuJo.Contracts.V1.Habits;
using Microsoft.AspNetCore.Mvc;

namespace BuJo.Web.Controllers;

[ApiController]
public sealed class HabitController : ControllerBase
{
    private const string TelegramIdHeader = "X-Telegram-Id";

    private readonly IUserService _userService;
    private readonly IHabitService _habitService;

    public HabitController(IUserService userService, IHabitService habitService)
    {
        _userService = userService;
        _habitService = habitService;
    }

    [HttpPost(ApiRoutesV1.Habits)]
    public async Task<ActionResult<HabitResponse>> CreateAsync(
        CreateHabitRequest request,
        CancellationToken ct)
    {
        var telegramId = GetTelegramId();
        if (telegramId is null)
            return BadRequest($"Заголовок {TelegramIdHeader} обязателен");

        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
            return NotFound($"Пользователь с TelegramId {telegramId} не найден");

        try
        {
            var habit = await _habitService.CreateAsync(new CreateHabitCommand(user.Id.Value, request.Name), ct);
            return Created($"{ApiRoutesV1.Habits}/{habit.Id}", habit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet(ApiRoutesV1.Habits)]
    public async Task<ActionResult<IReadOnlyList<HabitResponse>>> GetListAsync(
        [FromQuery] bool includeArchived,
        CancellationToken ct)
    {
        var telegramId = GetTelegramId();
        if (telegramId is null)
            return BadRequest($"Заголовок {TelegramIdHeader} обязателен");

        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
            return NotFound($"Пользователь с TelegramId {telegramId} не найден");

        var habits = await _habitService.GetListAsync(new GetHabitsQuery(user.Id.Value, includeArchived), ct);
        return Ok(habits);
    }

    /// <summary>
    /// Отметить выполнение привычки
    /// </summary>
    [HttpPost(ApiRoutesV1.HabitLog)]
    public async Task<ActionResult<HabitLogResponse>> LogAsync(
        Guid id,
        LogHabitRequest request,
        CancellationToken ct)
    {
        var telegramId = GetTelegramId();
        if (telegramId is null)
            return BadRequest($"Заголовок {TelegramIdHeader} обязателен");

        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
            return NotFound($"Пользователь с TelegramId {telegramId} не найден");

        var habit = await _habitService.GetByIdAsync(id, ct);
        if (habit is null)
            return NotFound($"Привычка {id} не найдена");

        if (habit.UserId != user.Id.Value)
            return Forbid();

        var date = request.Date is not null
            ? DateOnly.Parse(request.Date)
            : DateOnly.FromDateTime(DateTime.UtcNow);

        var isCompleted = request.IsCompleted ?? true;

        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest("Дата не может быть в будущем");

        try
        {
            var result = await _habitService.LogAsync(new LogHabitCommand(user.Id.Value, id, date, isCompleted), ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Получить статистику привычки
    /// </summary>
    [HttpGet(ApiRoutesV1.HabitStats)]
    public async Task<ActionResult<HabitStatsResponse>> GetStatsAsync(
        Guid id,
        [FromQuery] StatsPeriod? period,
        CancellationToken ct)
    {
        var telegramId = GetTelegramId();
        if (telegramId is null)
            return BadRequest($"Заголовок {TelegramIdHeader} обязателен");

        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
            return NotFound($"Пользователь с TelegramId {telegramId} не найден");

        var habit = await _habitService.GetByIdAsync(id, ct);
        if (habit is null)
            return NotFound($"Привычка {id} не найдена");

        if (habit.UserId != user.Id.Value)
            return Forbid();

        try
        {
            var stats = await _habitService.GetStatsAsync(
                new GetHabitStatsQuery(user.Id.Value, id, period ?? StatsPeriod.All), ct);
            return Ok(stats);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Получить логи привычек
    /// </summary>
    [HttpGet(ApiRoutesV1.HabitLogs)]
    public async Task<ActionResult<IReadOnlyList<HabitLogResponse>>> GetLogsAsync(
        [FromQuery] Guid? habitId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken ct)
    {
        var telegramId = GetTelegramId();
        if (telegramId is null)
            return BadRequest($"Заголовок {TelegramIdHeader} обязателен");

        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
            return NotFound($"Пользователь с TelegramId {telegramId} не найден");

        var logs = await _habitService.GetLogsAsync(
            new GetHabitLogsQuery(user.Id.Value, habitId, fromDate, toDate), ct);
        return Ok(logs);
    }

    private string? GetTelegramId()
    {
        var value = Request.Headers[TelegramIdHeader].ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
