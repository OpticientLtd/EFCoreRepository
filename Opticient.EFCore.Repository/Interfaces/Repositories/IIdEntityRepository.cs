using Opticient.EFCore.Repository.Interfaces.Entities;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Interfaces.Repositories;

/// <summary>
/// Represents a repository interface for entities that implement <see cref="IIdEntity{TKey}" />.
/// Provides methods for adding, updating, and reading entities by their unique identifier.
/// </summary>
/// <typeparam name="TEntity">The type of entity that extends <see cref="IIdEntity{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
public interface IIdEntityRepository<TEntity, in TKey> :
    IIdEntityReadRepository<TEntity, TKey>
    where TEntity : IIdEntity<TKey>
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of new entities to the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes the specified entity from the data store.
    /// </summary>
    /// <param name="entity">
    /// The entity instance to remove.
    /// </param>
    void Remove(TEntity entity);

    /// <summary>
    /// Asynchronously removes an entity identified by its primary key.
    /// </summary>
    /// <param name="primaryKeyId">
    /// The primary key value of the entity to remove.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous remove operation.
    /// </returns>
    Task RemoveAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);
}
