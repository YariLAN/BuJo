namespace BuJo.Common.Extensions.Configurations;

public interface IHaveConfigSection
{
    /// <summary>
    /// Ключ секции в конфиге
    /// </summary>
    static abstract string SectionName { get; }
}