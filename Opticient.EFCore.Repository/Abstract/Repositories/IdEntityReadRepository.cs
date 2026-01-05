using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Abstract.Entities;
using Opticient.EFCore.Repository.Interfaces.Repositories;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Abstract.Repositories;

/// <inheritdoc />
public abstract class IdEntityReadRepository<TEntity, TKey>(DbContext dbContext, ILogger<EntityBaseRepository<TEntity>> logger)
    : EntityBaseRepository<TEntity>(dbContext, logger), IIdEntityReadRepository<TEntity, TKey>
    where TEntity : IdEntity<TKey>
{
    #region VARIABLES

    #region PROTECTED

    protected override Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> DefaultOrderBy
        => o => o.OrderBy(entity => entity.Id);

    protected new ILogger<IdEntityReadRepository<TEntity, TKey>> Logger
        => base.Logger as ILogger<IdEntityReadRepository<TEntity, TKey>>;

    #endregion PROTECTED

    #endregion VARIABLES

    #region METHODS

    #region PUBLIC

    public virtual async Task<TEntity> GetAsync(bool readOnly, TKey primaryKeyId,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
    CancellationToken cancellationToken = default)
        => await base.FirstOrDefaultAsync(readOnly: readOnly,
            predicate: e => e.Id.Equals(primaryKeyId),
            includes: includes,
            orderBy: null,
            cancellationToken: cancellationToken);

    public virtual async Task<TProjectedType> GetProjectedAsync<TProjectedType>(TKey primaryKeyId,
        Expression<Func<TEntity, TProjectedType>> selector, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await base.GetProjectedAsync(selector: selector,
            predicate: e => e.Id.Equals(primaryKeyId),
            entityOrderBy: null,
            cancellationToken: cancellationToken);
    }

    public virtual async Task<TEntity> GetWithAllNavigationsAsync(bool readOnly, TKey primaryKeyId, CancellationToken cancellationToken = default)
        => await base.GetWithAllNavigationsAsync(readOnly: readOnly,
            predicate: e => e.Id.Equals(primaryKeyId),
            orderBy: null,
            cancellationToken: cancellationToken);

    public virtual async Task<bool> IdExistsAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
        => await base.AnyAsync(predicate: e => e.Id.Equals(primaryKeyId), cancellationToken: cancellationToken);

    public virtual async Task<TEntity> GetForDeleteAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
        => await GetAsync(readOnly: false, primaryKeyId: primaryKeyId, includes: null, cancellationToken: cancellationToken);

    #endregion PUBLIC

    #endregion METHODS
}
