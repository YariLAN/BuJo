using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuJo.TelegramBot.Handlers.Commands;

public sealed class StartCommandHandler(ITelegramBotClient botClient) : ICommandHandler
{
    public string Command => "/start";
    
    public async Task HandleAsync(Message message, CancellationToken ct)
        => await botClient.SendPoll(message.Chat.Id, "Что из списка было выполнено сегодня?", 
            new []
            {
                new InputPollOption("Зал"),
                new InputPollOption("ПМШ")
            }, false, PollType.Regular, true, cancellationToken: ct);

}