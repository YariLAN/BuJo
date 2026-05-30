using Ardalis.Specification;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

public interface IUserRepository : IRepositoryBase<User>
{
   
    Task<User?> GetBySpecAsync(ISpecification<User> specification, CancellationToken ct);
}