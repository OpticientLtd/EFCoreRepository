using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository
{
    public class Repository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly DbContext dbContext;
        private readonly DbSet<TEntity> _dbSet;
        private bool disposedValue = false;

        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = this.dbContext.Set<TEntity>();
        }
        public virtual async Task AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null,
             CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.AnyAsync(cancellationToken);
            else
                return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate,
             CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AllAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> ContainsAsync(TEntity entity,
             CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dbSet.ContainsAsync(entity, cancellationToken);
        }
        public virtual async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null,
             CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.LongCountAsync(cancellationToken);
            else
                return await _dbSet.LongCountAsync(predicate, cancellationToken);
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy == null)
                return await query.FirstOrDefaultAsync(cancellationToken);
            else
                return await orderBy(query).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy == null)
                return await query.LastOrDefaultAsync(cancellationToken);
            else
                return await orderBy(query).LastOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy == null)
                return await query.SingleOrDefaultAsync(cancellationToken);
            else
                return await orderBy(query).SingleOrDefaultAsync(cancellationToken);
        }
        public virtual async Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            await _dbSet.ForEachAsync(action, cancellationToken);
        }
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(int skipRecords = 0,
            int returnRecords = int.MaxValue,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            IEnumerable<string> includeProperties = null,
            CancellationToken cancellationToken = default)
        {
            if (skipRecords < 0) skipRecords = 0;

            IQueryable<TEntity> query = _dbSet;

            if (includeProperties != null)
                foreach (var property in includeProperties)
                    query = query.Include(property);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy == null)
                return await query.Skip(skipRecords).Take(returnRecords).ToArrayAsync(cancellationToken);
            else
                return await orderBy(query).Skip(skipRecords).Take(returnRecords).ToArrayAsync(cancellationToken);
        }

        public virtual async Task<TEntity> GetAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { primaryKeyId }, cancellationToken);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
        {
            var entity = await this.GetAsync(primaryKeyId, cancellationToken);
            if (entity != null)
                this.Remove(entity);
        }

        public virtual async Task<decimal?> SumAsync(Expression<Func<TEntity, decimal?>> sumPredicate,
            Expression<Func<TEntity, bool>> filterPredicate = null,
            CancellationToken cancellationToken = default)
        {
            if (sumPredicate == null)
                throw new ArgumentNullException(nameof(sumPredicate));

            if (filterPredicate == null)
                return await _dbSet.SumAsync(sumPredicate, cancellationToken);
            else
                return await _dbSet.Where(filterPredicate).SumAsync(sumPredicate, cancellationToken);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
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
    }
}
