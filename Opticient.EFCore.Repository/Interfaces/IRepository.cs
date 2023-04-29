namespace Opticient.EFCore.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<TEntity, TKey> :
        IReadRepository<TEntity, TKey>
        where TEntity : IDbIdEntity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);

        Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default);
    }
}
