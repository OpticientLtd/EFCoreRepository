using Microsoft.EntityFrameworkCore.Query;

using Opticient.EFCore.Repository.Interfaces.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Opticient.EFCore.Repository.Interfaces.Repositories;

/// <summary>
/// Represents a generic repository interface for entities that implement <see cref="IEntityBase"
/// />. Provides methods for data access and manipulation in a database context.
/// </summary>
/// <typeparam name="TEntity">The type of entity that extends <see cref="IEntityBase" />.</typeparam>
/// <remarks>
/// This interface defines methods for common operations such as querying, counting, and projecting
/// data from the database, with support for predicates, ordering, and navigation properties.
/// </remarks>
public interface IEntityBaseRepository<TEntity> : IBaseRepository
    where TEntity : IEntityBase
{
    /// <summary>
    /// Checks if all entities match a given predicate.
    /// </summary>
    /// <param name="predicate">The condition that the entities must meet.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean result.</returns>
    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entities match a given predicate.
    /// </summary>
    /// <param name="predicate">The optional condition to filter the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean result.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if the specified entity is contained in the repository.
    /// </summary>
    /// <param name="entity">The entity to check for existence in the repository.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean result.</returns>
    Task<bool> ContainsAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the first entity matching a given predicate or default if none found.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the first matching entity or default.
    /// </returns>
    Task<TEntity> FirstOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the last entity matching a given predicate or default if none found.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the last matching entity or default.
    /// </returns>
    Task<TEntity> LastOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity matching a given predicate or default if none found.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the single matching entity or default.
    /// </returns>
    Task<TEntity> SingleOrDefaultAsync(bool readOnly, Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities matching a given predicate, with pagination and ordering.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="skipRecords">The number of records to skip for pagination.</param>
    /// <param name="returnRecords">The maximum number of records to return.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="includes">The related entities to include in the query.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a collection of matching entities.
    /// </returns>
    Task<IEnumerable<TEntity>> GetAllAsync(bool readOnly, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects and retrieves all entities into a specified type.
    /// </summary>
    /// <typeparam name="TProjectedType">The type to which the entities will be projected.</typeparam>
    /// <param name="selector">The projection to apply to each entity.</param>
    /// <param name="skipRecords">The number of records to skip for pagination.</param>
    /// <param name="returnRecords">The maximum number of records to return.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="entityOrderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a collection of projected entities.
    /// </returns>
    Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects and retrieves all entities into a specified type, with pagination and ordering.
    /// </summary>
    /// <typeparam name="TProjectedType">The type to which the entities will be projected.</typeparam>
    /// <param name="selector">The projection to apply to each entity.</param>
    /// <param name="skipRecords">The number of records to skip for pagination.</param>
    /// <param name="returnRecords">The maximum number of records to return.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="projectedTypeOrderBy">The order in which to return the projected entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a collection of projected entities.
    /// </returns>
    Task<IEnumerable<TProjectedType>> GetAllProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> projectedTypeOrderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects and retrieves distinct entities into a specified type.
    /// </summary>
    /// <typeparam name="TProjectedType">The type to which the entities will be projected.</typeparam>
    /// <param name="selector">The projection to apply to each entity.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="projectedTypeOrderBy">The order in which to return the projected entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a collection of distinct
    /// projected entities.
    /// </returns>
    Task<IEnumerable<TProjectedType>> GetAllDistinctProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TProjectedType>, IOrderedQueryable<TProjectedType>> projectedTypeOrderBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities including all navigation properties, with pagination and ordering.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="skipRecords">The number of records to skip for pagination.</param>
    /// <param name="returnRecords">The maximum number of records to return.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a collection of matching entities.
    /// </returns>
    Task<IEnumerable<TEntity>> GetAllWithAllNavigationsAsync(bool readOnly, int skipRecords = 0, int returnRecords = int.MaxValue,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity including all navigation properties.
    /// </summary>
    /// <param name="readOnly">Indicates whether the operation is read-only.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="orderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the single matching entity or default.
    /// </returns>
    Task<TEntity> GetWithAllNavigationsAsync(bool readOnly,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Projects and retrieves a single entity into a specified type.
    /// </summary>
    /// <typeparam name="TProjectedType">The type to which the entity will be projected.</typeparam>
    /// <param name="selector">The projection to apply to the entity.</param>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="entityOrderBy">The order in which to return the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the projected entity or default.
    /// </returns>
    Task<TProjectedType> GetProjectedAsync<TProjectedType>(Expression<Func<TEntity, TProjectedType>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> entityOrderBy = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities that match a given predicate.
    /// </summary>
    /// <param name="predicate">The condition to filter the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the count of matching entities.
    /// </returns>
    Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the sum of a specified property for entities that match a given filter.
    /// </summary>
    /// <param name="sumPredicate">The property to sum.</param>
    /// <param name="filterPredicate">The optional condition to filter the entities.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the sum of the specified property.
    /// </returns>
    Task<double?> SumAsync(Expression<Func<TEntity, double?>> sumPredicate, Expression<Func<TEntity, bool>> filterPredicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an action on each entity in the repository.
    /// </summary>
    /// <param name="action">The action to perform on each entity.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ForEachAsync(Action<TEntity> action, CancellationToken cancellationToken = default);
}