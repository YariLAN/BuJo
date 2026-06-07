using BuJo.Common.Extensions.Configurations;

namespace BuJo.Host;

public sealed class HostOptions : IHostOptions
{
    public static string SectionName => "Hosting";
    
    public string? Name { get; set; }
}