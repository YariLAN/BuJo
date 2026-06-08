using Ardalis.Specification.EntityFrameworkCore;
using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using Microsoft.EntityFrameworkCore;

namespace BuJo.Data.Repositories.Accounting;

public class UserBotStateRepository : RepositoryBase<UserBotState>, IUserBotStateRepository
{
    private readonly DataContext _dataContext;

    public UserBotStateRepository(DataContext dataContext) : base(dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<UserBotState?> GetByUserIdAsync(Guid userId, long chatId, CancellationToken ct)
        => _dataContext.UserBotStates.FirstOrDefaultAsync(s => s.UserId == userId && s.ChatId == chatId, ct);
}
