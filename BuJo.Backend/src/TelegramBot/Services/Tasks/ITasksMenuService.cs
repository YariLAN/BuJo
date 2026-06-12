namespace BuJo.TelegramBot.Services.Tasks;

public interface ITasksMenuService
{
    Task OpenCreateAsync(Guid userId, long chatId, CancellationToken ct);
}