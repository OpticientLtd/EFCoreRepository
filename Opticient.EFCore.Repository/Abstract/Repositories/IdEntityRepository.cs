using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Abstract.Entities;
using Opticient.EFCore.Repository.Interfaces.Repositories;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Abstract.Repositories;

/// <inheritdoc />
public abstract class IdEntityRepository<TEntity, TKey>(DbContext dbContext, ILogger<IdEntityRepository<TEntity, TKey>> logger)
    : IdEntityReadRepository<TEntity, TKey>(dbContext, logger),
    IIdEntityRepository<TEntity, TKey>
    where TEntity : IdEntity<TKey>
{
    #region VARIABLES

    #region PROTECTED

    protected new ILogger<IdEntityRepository<TEntity, TKey>> Logger
        => base.Logger as ILogger<IdEntityRepository<TEntity, TKey>>;

    #endregion PROTECTED

    #endregion VARIABLES

    #region METHODS

    #region PUBLIC

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await dbSet.AddAsync(entity, cancellationToken);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await dbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Remove(TEntity entity)
        => dbSet.Remove(entity);

    public async Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
    {
        var entity = await base.GetForDeleteAsync(primaryKeyId, cancellationToken);
        if (entity != null)
        {
            this.Remove(entity);
        }
    }

    public virtual void Update(TEntity entity)
        => dbSet.Attach(entity).State = EntityState.Modified;

    #endregion PUBLIC

    #endregion METHODS
}
