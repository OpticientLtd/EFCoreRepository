namespace Opticient.EFCore.Repository.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Query;

public interface IReadRepository<TEntity, TKey> : IDisposable
    where TEntity : IDbIdEntity<TKey>
{
    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
    Task<TEntity> FirstOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking, int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector, int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> orderBy = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllWithAllNavigationsAsync(bool asNoTracking, int skipRecords = 0, int returnRecords = int.MaxValue, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(bool asNoTracking, TKey primaryKeyId, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, CancellationToken cancellationToken = default);
    Task<TProjectedType> GetProjectedAsync<TProjectedType>(TKey primaryKeyId, Expression<Func<TEntity, TProjectedType>> selector, CancellationToken cancellationToken = default);
    Task<TEntity> GetWithAllNavigationsAsync(bool asNoTracking, TKey primaryKeyId, CancellationToken cancellationToken = default);
    Task<TEntity> LastOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
    Task<TEntity> SingleOrDefaultAsync(bool asNoTracking, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);
    Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> sumPredicate, Expression<Func<TEntity, bool>> filterPredicate = null, CancellationToken cancellationToken = default);
}
