using Microsoft.EntityFrameworkCore.Query;

using Opticient.EFCore.Repository.Interfaces.Entities;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Interfaces.Repositories;

/// <summary>
/// Represents a repository interface for entities that implement <see cref="IIdEntity{TKey}" />.
/// Provides methods for retrieving entities by their unique identifier, along with other operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity that extends <see cref="IIdEntity{TKey}" />.</typeparam>
/// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
public interface IIdEntityReadRepository<TEntity, in TKey> :
    IEntityBaseRepository<TEntity>
    where TEntity : IIdEntity<TKey>
{
    /// <summary>
    /// Retrieves an entity by its primary key.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="primaryKeyId">The unique identifier of the entity.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the entity with the specified identifier.
    /// </returns>
    Task<TEntity> GetAsync(bool readOnly, TKey primaryKeyId,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects and retrieves an entity by its primary key into a specified type.
    /// </summary>
    /// <typeparam name="TProjectedType">The type to which the entity will be projected.</typeparam>
    /// <param name="primaryKeyId">The unique identifier of the entity.</param>
    /// <param name="selector">The projection to apply to the entity.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the projected entity.
    /// </returns>
    Task<TProjectedType> GetProjectedAsync<TProjectedType>(TKey primaryKeyId, Expression<Func<TEntity, TProjectedType>> selector,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity by its primary key, including all navigation properties.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="primaryKeyId">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the entity with all navigations.
    /// </returns>
    Task<TEntity> GetWithAllNavigationsAsync(bool readOnly, TKey primaryKeyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an entity with the specified primary key exists.
    /// </summary>
    /// <param name="primaryKeyId">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a boolean indicating if the
    /// entity exists.
    /// </returns>
    Task<bool> IdExistsAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity for deletion by its primary key.
    /// </summary>
    /// <param name="primaryKeyId">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the entity to be deleted.
    /// </returns>
    Task<TEntity> GetForDeleteAsync(TKey primaryKeyId, CancellationToken cancellationToken = default);
}