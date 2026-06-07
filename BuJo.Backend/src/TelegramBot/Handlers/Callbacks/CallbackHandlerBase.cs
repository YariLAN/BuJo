using BuJo.Application.Accounting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuJo.TelegramBot.Handlers.Callbacks;

/// <summary>
/// Базовый класс для обработки callback вызовов
/// </summary>
public abstract class CallbackHandlerBase : ICallbackHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;
    private readonly ILogger _logger;

    protected CallbackHandlerBase(
        ITelegramBotClient botClient, 
        IUserService userService, 
        ILogger logger)
    {
        _botClient = botClient;
        _userService = userService;
        _logger = logger;
    }

    public abstract string Prefix { get; }

    public async Task HandleAsync(CallbackQuery callback, CancellationToken ct)
    {
        try
        {
            await DispatchAsync(callback, ct);
        }
        finally
        {
            try
            {
                await _botClient.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to answer callback query {CallbackId}", callback.Id);
            }
        }
    }

    protected abstract Task HandleCallbackAsync(
        Guid userId,
        CallbackQuery callbackQuery,
        CancellationToken ct = default);

    private async Task DispatchAsync(CallbackQuery callback, CancellationToken ct)
    {
        if (callback.Message is null || callback.Data is null)
        {
            _logger.LogWarning("Callback {CallbackId} has no message or data", callback.Id);
            return;
        }
        
        var telegramId = callback.From.Id.ToString();
        var user = await _userService.GetOrDefaultAsync(new GetUserQuery(null, TelegramId: telegramId), ct);
        if (user?.Id is null)
        {
            await _botClient.AnswerCallbackQuery(
                callback.Id,
                text: "Сначала отправь /start",
                showAlert: true,
                cancellationToken: ct);
            return;
        }
        
        await HandleCallbackAsync(user.Id.Value, callback, ct);
    }
}