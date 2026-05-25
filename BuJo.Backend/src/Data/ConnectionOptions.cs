using BuJo.Common.Extensions.Configurations;

namespace BuJo.Data;

public sealed class ConnectionOptions : IHaveConfigSection
{
    public static string SectionName => "Database";
    
    public string? ConnectionString { get; set; }
}