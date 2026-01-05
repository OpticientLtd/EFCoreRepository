using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Abstract.Entities;
using Opticient.EFCore.Repository.Extensions;
using Opticient.EFCore.Repository.Interfaces.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Abstract.Repositories;

/// <inheritdoc />
public abstract class EntityBaseRepository<TEntity> : BaseRepository, IEntityBaseRepository<TEntity>
    where TEntity : EntityBase
{
    #region CONSTRUCTORS

    protected EntityBaseRepository(DbContext dbContext, ILogger<EntityBaseRepository<TEntity>> logger)
        : base(dbContext, logger)
    {
        ArgumentNullException.ThrowIfNull(DbContext.Model.FindEntityType(typeof(TEntity)), "Invalid TEntity");
        dbSet = DbContext.Set<TEntity>();
        EntityTypeName = typeof(TEntity).Name;
    }

    #endregion CONSTRUCTORS

    #region VARIABLES

    #region PRIVATE

    #endregion PRIVATE

    #region PROTECTED

    protected readonly string EntityTypeName;

    protected readonly DbSet<TEntity> dbSet;

    protected new ILogger<EntityBaseRepository<TEntity>> Logger
        => base.Logger as ILogger<EntityBaseRepository<TEntity>>;

    protected virtual Expression<Func<TEntity, bool>> DefaultPredicate
        => Extensions.ExpressionExtensions.True<TEntity>();

    protected virtual Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> DefaultOrderBy => null;

    #endregion PROTECTED

    #endregion VARIABLES

    #region METHODS

    #region PRIVATE

    private Expression<Func<TEntity, bool>> GetCombinedFilterPredicate(Expression<Func<TEntity, bool>> predicate = null)
    {
        /*
            predicate   DefaultPredicate    Final Predicate
        ------------------------------------------------------------------------------------
            null	    null	            Exception
            null	    not null	        DefaultPredicate
            not null	null	            predicate
            not null	not null	        predicate + DefaultPredicate
        ------------------------------------------------------------------------------------

         */

        var preCondition = DefaultPredicate != null;

        return predicate == null
            ? preCondition ? DefaultPredicate : null
            : preCondition ? predicate.And(DefaultPredicate) : predicate;
    }

    #endregion PRIVATE

    #region PROTECTED

    protected virtual IQueryable<TEntity> GetBasicFilteredQuery(bool readOnly = false,
        Expression<Func<TEntity, bool>> predicate = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (readOnly)
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }
        var combinedPredicate = GetCombinedFilterPredicate(predicate: predicate);
        if (combinedPredicate != null)
        {
            query = query.Where(combinedPredicate);
        }
        return query;
    }

    protected virtual IQueryable<TEntity> GetOrderedQuery(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        => orderBy == null ? (DefaultOrderBy == null ? query : DefaultOrderBy(query)) : orderBy(query);

    protected virtual IQueryable<TEntity> GetFilteredQuery(bool readOnly = false,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        IEnumerable<string> includeProperties = null)
    {
        IQueryable<TEntity> query = GetBasicFilteredQuery(readOnly: readOnly, predicate: predicate);

        if (!includeProperties.IsNullOrEmpty())
        {
            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));
        }

        return GetOrderedQuery(query: query, orderBy: orderBy);
    }

    protected virtual IQueryable<TEntity> GetFilteredQuery(bool readOnly = false,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
    {
        IQueryable<TEntity> query = GetBasicFilteredQuery(readOnly: readOnly, predicate: predicate);

        if (includes != null)
        {
            query = includes(query);
        }

        return GetOrderedQuery(query: query, orderBy: orderBy);
    }

    protected virtual IQueryable<TProjectedType> GetFilteredProjectedQuery<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector = null,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> projectedTypeOrderBy = null)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        if (entityOrderBy != null && projectedTypeOrderBy != null)
        {
            throw new InvalidOperationException("Entity and Projected Order by can not be together.");
        }

        IQueryable<TEntity> query = GetBasicFilteredQuery(readOnly: true, predicate: predicate);

        if (entityOrderBy == null)
        {
            if (projectedTypeOrderBy == null && DefaultOrderBy != null)
            {
                query = DefaultOrderBy(query);
            }
        }
        else
        {
            query = GetOrderedQuery(query: query, orderBy: entityOrderBy);
        }
        IQueryable<TProjectedType> projectedQuery = query.Select(selector);

        if (projectedTypeOrderBy != null)
        {
            projectedQuery = projectedTypeOrderBy(projectedQuery);
        }

        return projectedQuery;
    }

    protected async Task<IEnumerable<TProjectedType>> GetDataAsync<TProjectedType>(IQueryable<TProjectedType> query, int skipRecords = 0,
        int returnRecords = int.MaxValue, CancellationToken cancellationToken = default)
    {
        if (skipRecords < 0)
        {
            skipRecords = 0;
        }

        if (returnRecords < 1)
        {
            returnRecords = int.MaxValue;
        }

        query = query.Skip(skipRecords).Take(returnRecords);
        Logger.LogDebug("GetDataAsync(Entity: {Entity}): (Query: {Query})", EntityTypeName, query.ToQueryString());
        try
        {
            return await query.ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GetDataAsync(Entity: {Entity}): (Query: {Query})", EntityTypeName, query.ToQueryString());
            throw;
        }
    }

    protected IQueryable<TEntity> GetFilteredQueryWithAllIncludesAsString(bool readOnly,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        => GetFilteredQuery(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy,
            includeProperties: DbContext.GetNavigationProperties<TEntity>());

    #endregion PROTECTED

    #region PUBLIC

    public virtual async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            predicate = GetCombinedFilterPredicate(predicate: predicate);
            Logger.LogDebug("AllAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
            return predicate == null ? throw new ArgumentNullException(nameof(predicate)) : await dbSet.AllAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "AllAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
            throw;
        }
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            predicate = GetCombinedFilterPredicate(predicate: predicate);
            Logger.LogDebug("AnyAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
            return predicate == null ? await dbSet.AnyAsync(cancellationToken) : await dbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "AnyAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
            throw;
        }
    }

    public virtual async Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("ContainsAsync(Entity: {Entity})", typeof(TEntity).Name);
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        try
        {
            return await dbSet.ContainsAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ContainsAsync(Entity: {Entity})", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy,
            includes: includes)
            .FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<TEntity> LastOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy,
            includes: includes)
            .LastOrDefaultAsync(cancellationToken);

    public virtual async Task<TEntity> SingleOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy,
            includes: includes)
            .SingleOrDefaultAsync(cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool readOnly, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        CancellationToken cancellationToken = default)
        => await GetDataAsync(query: GetFilteredQuery(readOnly: readOnly,
                predicate: predicate,
                orderBy: orderBy,
                includes: includes),
            skipRecords: skipRecords,
            returnRecords: returnRecords,
            cancellationToken: cancellationToken);

    public virtual async Task<TProjectedType> GetProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await GetFilteredProjectedQuery(selector: selector,
            predicate: predicate,
            entityOrderBy: entityOrderBy)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await GetDataAsync(query: GetFilteredProjectedQuery(
                selector: selector,
                predicate: predicate,
                entityOrderBy: entityOrderBy,
                projectedTypeOrderBy: null),
            skipRecords: skipRecords,
            returnRecords: returnRecords,
            cancellationToken: cancellationToken);
    }

    public virtual async Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> projectedTypeOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await GetDataAsync(
            query: GetFilteredProjectedQuery(selector: selector,
                predicate: predicate,
                entityOrderBy: null,
                projectedTypeOrderBy: projectedTypeOrderBy),
            skipRecords: skipRecords,
            returnRecords: returnRecords,
            cancellationToken: cancellationToken);
    }

    public virtual async Task<IEnumerable<TProjectedType>> GetAllDistinctProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> projectedTypeOrderBy = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await GetDataAsync(
            query: GetFilteredProjectedQuery(selector: selector,
                predicate: predicate,
                entityOrderBy: null,
                projectedTypeOrderBy: projectedTypeOrderBy).Distinct(),
            skipRecords: 0,
            returnRecords: int.MaxValue,
            cancellationToken: cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllWithAllNavigationsAsync(bool readOnly, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetDataAsync(query: GetFilteredQueryWithAllIncludesAsString(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy),
            skipRecords: skipRecords,
            returnRecords: returnRecords,
            cancellationToken: cancellationToken);

    public virtual async Task<TEntity> GetWithAllNavigationsAsync(bool readOnly,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQueryWithAllIncludesAsString(readOnly: readOnly,
            predicate: predicate,
            orderBy: orderBy)
            .FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("CountAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
        try
        {
            predicate = GetCombinedFilterPredicate(predicate: predicate);
            return predicate == null ? await dbSet.LongCountAsync(cancellationToken) : await dbSet.LongCountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "CountAsync(Entity: {Entity}): (Predicate: {Predicate})", EntityTypeName, predicate?.ToReadableString());
            throw;
        }
    }

    public virtual async Task<double?> SumAsync(Expression<Func<TEntity, double?>> sumPredicate,
        Expression<Func<TEntity, bool>> filterPredicate = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sumPredicate, nameof(sumPredicate));
        try
        {
            Logger.LogDebug("SumAsync(Entity: {Entity}): (FilterPredicate: {FilterPredicate})", EntityTypeName, filterPredicate?.ToReadableString());
            filterPredicate = GetCombinedFilterPredicate(predicate: filterPredicate);
            return filterPredicate == null
                ? await dbSet.SumAsync(sumPredicate, cancellationToken)
                : await dbSet.Where(filterPredicate).SumAsync(sumPredicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "SumAsync(Entity: {Entity}): (FilterPredicate: {FilterPredicate})", EntityTypeName, filterPredicate?.ToReadableString());
            throw;
        }
    }

    public virtual async Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));
        await dbSet.ForEachAsync(action, cancellationToken);
    }

    #endregion PUBLIC

    #endregion METHODS
}
