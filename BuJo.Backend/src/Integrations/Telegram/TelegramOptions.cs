using BuJo.Common.Extensions.Configurations;

namespace BuJo.Integrations.Telegram;

public sealed class TelegramOptions : IHaveConfigSection
{
    public static string SectionName => "Telegram";
    
    public string? Token { get; set; }

    public string TokenRequired => Token ?? throw new ArgumentNullException();
}