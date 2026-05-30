using BuJo.Common.Extensions.Configurations;

namespace BuJo.TelegramBot;

public sealed class TelegramOptions : IHaveConfigSection
{
    public static string SectionName => "Telegram";
    
    public string? Token { get; init; }

    public string TokenRequired => Token ?? throw new ArgumentNullException();
}