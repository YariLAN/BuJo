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

    private string? GetTelegramId()
    {
        var value = Request.Headers[TelegramIdHeader].ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
