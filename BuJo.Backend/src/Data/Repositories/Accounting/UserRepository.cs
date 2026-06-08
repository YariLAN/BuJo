using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using BuJo.Application.Accounting;
using BuJo.Application.Accounting.Abstractions;
using BuJo.Domain.Accounting;
using Microsoft.EntityFrameworkCore;

namespace BuJo.Data.Repositories.Accounting;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly DataContext _dataContext;

    public UserRepository(DataContext dataContext) : base(dataContext)
    {
        _dataContext = dataContext;
    }

    private IQueryable<User> BaseQuery => _dataContext.Users
        .Include(u => u.Tasks)
        .Include(u => u.Habits);

    protected override IQueryable<User> ApplySpecification(
        ISpecification<User> specification, 
        bool evaluateCriteriaOnly = false)
    {
        return SpecificationEvaluator.GetQuery(BaseQuery, specification, evaluateCriteriaOnly);
    }

    public Task<User?> GetBySpecAsync(ISpecification<User> specification, CancellationToken ct)
        => FirstOrDefaultAsync(specification, ct);
}