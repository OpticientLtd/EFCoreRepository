using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Remove(TEntity entity);
        Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync(int skipRecords = 0,
            int returnRecords = int.MaxValue,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null,
            CancellationToken cancellationToken = default);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null,
            CancellationToken cancellationToken = default);

        Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
             CancellationToken cancellationToken = default);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null,
            CancellationToken cancellationToken = default);

        Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> sumPredicate,
            Expression<Func<TEntity, bool>> filterPredicate = null,
            CancellationToken cancellationToken = default);

        Task<bool> ContainsAsync(TEntity entity,
             CancellationToken cancellationToken = default);

        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default);

        Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default);
    }
}
