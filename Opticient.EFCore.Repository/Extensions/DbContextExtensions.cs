using Microsoft.EntityFrameworkCore;

using Opticient.EFCore.Repository.Abstract.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Opticient.EFCore.Repository.Extensions;

public static class DbContextExtensions
{
    /// <summary>
    /// Retrieves the names of all reference (non-collection) navigation properties
    /// defined for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The entity type whose navigation properties are inspected.
    /// </typeparam>
    /// <param name="dbContext">
    /// The <see cref="DbContext"/> used to access the EF Core model metadata.
    /// </param>
    /// <returns>
    /// A collection of navigation property names that represent
    /// reference relationships (excluding collection navigations).
    /// </returns>
    public static IEnumerable<string> GetNavigationProperties<TEntity>(this DbContext dbContext)
        where TEntity : EntityBase
    {
        var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
        return entityType?
            .GetNavigations()?
            .Where(p => !p.IsCollection)?
            .Select(x => x.Name)?
            .ToArray() ?? [];
    }
}
