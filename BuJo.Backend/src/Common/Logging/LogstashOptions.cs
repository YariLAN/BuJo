using BuJo.Common.Extensions.Configurations;

namespace BuJo.Common.Logging;

public sealed class LogstashOptions : IHaveConfigSection
{
    public static string SectionName => "Logstash";

    public string? ServerUrl { get; set; }

    public string RequiredServerUrl => ServerUrl ?? throw new ArgumentNullException();
}
