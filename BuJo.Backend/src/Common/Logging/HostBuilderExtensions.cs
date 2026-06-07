using BuJo.Common.Extensions.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;

namespace BuJo.Common.Logging;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseLogger<THostOptions>(this IHostBuilder builder, 
        Action<HostBuilderContext, LoggerConfiguration>? configure = null)
        where THostOptions : class, IHostOptions
    {
        builder.UseSerilog((context, configuration) =>
        {
            var hostOptions = context.Configuration.GetRequiredSection(THostOptions.SectionName).Get<THostOptions>()
                              ?? throw new ArgumentNullException();
            
            var lostashOptions = context.Configuration.GetRequiredSection(LogstashOptions.SectionName)
                                     .Get<LogstashOptions>()
                                 ?? throw new ArgumentNullException();

            configuration
                .ConfigureEnrichers()
                .ConfigureSinks(hostOptions, lostashOptions, false)
                .ReadFrom.Configuration(context.Configuration);
            
            configure?.Invoke(context, configuration);
        });

        return builder;
    }
    
    private static LoggerConfiguration ConfigureEnrichers(this LoggerConfiguration configuration)
        => configuration
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName();
    
    private static LoggerConfiguration ConfigureSinks(
        this LoggerConfiguration configuration,
        IHostOptions hostOptions,
        LogstashOptions logstashOptions,
        bool isDevelopment)
        => isDevelopment
            ? configuration
                .WriteTo.Async(configure => configure.Console())
            : configuration
                .WriteTo.Async(configure => configure.Console())
                .WriteTo.Async(configure => configure
                    .DurableHttpUsingFileSizeRolledBuffers(
                        requestUri: logstashOptions.RequiredServerUrl,
                        batchFormatter: new ArrayBatchFormatter(),
                        textFormatter: new JsonLogFormatter(hostOptions.RequiredName)));
}