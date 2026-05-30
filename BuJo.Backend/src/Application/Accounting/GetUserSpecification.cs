using Ardalis.Specification;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

public sealed class GetUserSpecification : Specification<User>
{
    public GetUserSpecification(GetUserQuery query)
    {
        Query.Where(u => u.Id == query.UserId, query.UserId is not null);
        
        Query.Where(u => u.TelegramId == query.TelegramId, query.TelegramId is not null);

        Query.Where(u => u.Name == query.UserName, query.UserName is not null);
    }
}