using BuJo.Application;
using BuJo.Common.Logging;
using BuJo.Data;
using BuJo.TelegramBot;
using BuJo.Web;
using Scalar.AspNetCore;
using Serilog;

namespace BuJo.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt")
            .CreateLogger();
        
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddWeb()
            .AddApplication()
            .AddData(builder.Configuration)
            .AddTelegramBot(builder.Configuration);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Host.UseLogger<HostOptions>();
        
        var app = builder.Build();

        try
        {
            Log.Information("Запуск приложения");
            
            await MigrateDatabase(app);
            
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseHttpsRedirection();
            
            
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Приложение неожиданно остановилось");
            throw;
        }
        finally
        {
            Log.Information("Остановка приложения...");
            await Log.CloseAndFlushAsync();
        }
    }
    
    private static async Task MigrateDatabase(IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();
        await using var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dataContext.MigrateAsync();
    }
}