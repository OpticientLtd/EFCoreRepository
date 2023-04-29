namespace Opticient.EFCore.Repository.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Opticient.EFCore.Repository.Interfaces;

    public abstract class Repository<TEntity, TKey> : ReadRepository<TEntity, TKey>,
        IRepository<TEntity, TKey>
        where TEntity : DbIdEntity<TKey>
    {
        public Repository(DbContext dbContext) : base(dbContext)
        {

        }
        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            await dbSet.ForEachAsync(action, cancellationToken);
        }

        public virtual void Remove(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public async Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default)
        {
            var entity = await base.GetAsync(false, primaryKeyId, null, cancellationToken);
            if (entity != null)
            {
                this.Remove(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            dbSet.Attach(entity).State = EntityState.Modified;
        }
    }
}
