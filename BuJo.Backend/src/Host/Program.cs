using System.Diagnostics;
using BuJo.Data;
using BuJo.Integrations.Telegram;
using BuJo.Web;
using Scalar.AspNetCore;

namespace BuJo.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddData(builder.Configuration)
            .AddWeb()
            .AddTelegram(builder.Configuration);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await MigrateDatabase(app);
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        await app.RunAsync();
    }
    
    private static async Task MigrateDatabase(IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();
        await using var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dataContext.MigrateAsync();
    }
}