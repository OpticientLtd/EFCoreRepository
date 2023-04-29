namespace Opticient.EFCore.Repository.Abstract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Opticient.EFCore.Repository.Interfaces;

public abstract class ReadRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>
    where TEntity : DbIdEntity<TKey>
{
    protected readonly DbContext dbContext;
    protected internal readonly DbSet<TEntity> dbSet;
    private bool _isDisposed = false;

    public ReadRepository(DbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        ArgumentNullException.ThrowIfNull(this.dbContext.Model.FindEntityType(typeof(TEntity)), "Invalid TEntity");
        dbSet = this.dbContext.Set<TEntity>();
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await dbSet.AnyAsync(cancellationToken);
        }
        else
        {
            return await dbSet.AnyAsync(predicate, cancellationToken);
        }
    }

    public virtual async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        return await dbSet.AllAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return await dbSet.ContainsAsync(entity, cancellationToken);
    }

    public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await dbSet.LongCountAsync(cancellationToken);
        }
        else
        {
            return await dbSet.LongCountAsync(predicate, cancellationToken);
        }
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(asNoTracking, predicate, orderBy, includes).FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<TEntity> LastOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(asNoTracking, predicate, orderBy, includes).LastOrDefaultAsync(cancellationToken);

    public virtual async Task<TEntity> SingleOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
        => await GetFilteredQuery(asNoTracking, predicate, orderBy, includes).SingleOrDefaultAsync(cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllWithAllNavigationsAsync(bool asNoTracking, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default)
            => await GetAllAsync(asNoTracking, skipRecords, returnRecords, predicate, orderBy,
                    includeProperties: this.dbContext.GetNavigationProperties<TEntity, TKey>(), cancellationToken);

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, CancellationToken cancellationToken = default)
        => await GetDataAsync<TEntity>(GetFilteredQuery(predicate: predicate, orderBy: orderBy, asNoTracking: asNoTracking, includes: includes), skipRecords, returnRecords, cancellationToken);

    public virtual async Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> orderBy = null, CancellationToken cancellationToken = default)
    {

        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        return await GetDataAsync<TProjectedType>(GetFilteredProjectedQuery<TProjectedType>(true, selector, predicate, orderBy),
            skipRecords, returnRecords, cancellationToken);
    }

    public virtual async Task<TEntity> GetAsync(bool asNoTracking, TKey primaryKeyId,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, CancellationToken cancellationToken = default)
        => await GetFilteredQuery(predicate: e => e.Id.Equals(primaryKeyId), asNoTracking: asNoTracking, includes: includes).FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<TEntity> GetWithAllNavigationsAsync(bool asNoTracking, TKey primaryKeyId, CancellationToken cancellationToken = default)
        => await GetFilteredQuery(asNoTracking: asNoTracking, predicate: e => e.Id.Equals(primaryKeyId), orderBy: null, includeProperties: this.dbContext.GetNavigationProperties<TEntity, TKey>()).FirstOrDefaultAsync(cancellationToken);

    public virtual async Task<TProjectedType> GetProjectedAsync<TProjectedType>(TKey primaryKeyId,
        Expression<Func<TEntity, TProjectedType>> selector, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        return await GetFilteredProjectedQuery<TProjectedType>(true, selector, e => e.Id.Equals(primaryKeyId)).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> sumPredicate,
        Expression<Func<TEntity, bool>> filterPredicate = null, CancellationToken cancellationToken = default)
    {
        if (sumPredicate == null)
        {
            throw new ArgumentNullException(nameof(sumPredicate));
        }

        if (filterPredicate == null)
        {
            return await dbSet.SumAsync(sumPredicate, cancellationToken);
        }
        else
        {
            return await dbSet.Where(filterPredicate).SumAsync(sumPredicate, cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                dbContext.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Repository()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private IQueryable<TEntity> GetFilteredQuery(bool asNoTracking = false,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        IEnumerable<string> includeProperties = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includeProperties != null && includeProperties.Any())
        {
            foreach (string property in includeProperties)
            {
                query = query.Include(property);
            }
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    private IQueryable<TEntity> GetFilteredQuery(bool asNoTracking = false,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includes != null)
        {
            query = includes(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    private IQueryable<TProjectedType> GetFilteredProjectedQuery<TProjectedType>(bool asNoTracking = false,
        Expression<Func<TEntity, TProjectedType>> selector = null,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> orderBy = null)
    {
        IQueryable<TEntity> query = dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        IQueryable<TProjectedType> projectedQuery = null;
        if (selector != null)
        {
            projectedQuery = query.Select(selector);
        }
        else
        {
            projectedQuery = (IQueryable<TProjectedType>)query;
        }

        if (orderBy != null)
        {
            projectedQuery = orderBy(projectedQuery);
        }

        return projectedQuery;
    }

    private async Task<IEnumerable<TProjectedType>> GetDataAsync<TProjectedType>(IQueryable<TProjectedType> query, int skipRecords = 0,
        int returnRecords = int.MaxValue, CancellationToken cancellationToken = default)
    {
        if (skipRecords < 0)
        {
            skipRecords = 0;
        }

        if (returnRecords < 0)
        {
            returnRecords = int.MaxValue;
        }

        return await query.Skip(skipRecords).Take(returnRecords).ToArrayAsync(cancellationToken);
    }

    private async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        IEnumerable<string> includeProperties = null, CancellationToken cancellationToken = default)
        => await GetDataAsync(GetFilteredQuery(asNoTracking, predicate, orderBy, includeProperties), skipRecords, returnRecords, cancellationToken);
}
