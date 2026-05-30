using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuJo.Integrations.Telegram.Contracts;

public sealed class TelegramUpdate
{
    public long UpdateId { get; set; }
    
    public Type Type { get; set; }
    
    public Message? Message { get; set; }
}

public enum Type
{
    Unknown = 0,
    Message,
}