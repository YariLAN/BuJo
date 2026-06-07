namespace BuJo.Common.Extensions.Configurations;

/// <summary>
/// Настройки хоста
/// </summary>
public interface IHostOptions : IHaveConfigSection
{
    static abstract string IHaveConfigSection.SectionName { get; }

    /// <summary>
    /// Имя сервиса
    /// </summary>
    public string? Name { get; set; }

    public string RequiredName => Name ?? throw new ArgumentNullException();
}