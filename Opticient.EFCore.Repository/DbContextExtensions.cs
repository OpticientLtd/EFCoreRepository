namespace Opticient.EFCore.Repository;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Opticient.EFCore.Repository.Abstract;

public static class DbContextExtensions
{
    public static IEnumerable<string> GetNavigationProperties<TEntity, TKey>(this DbContext dbContext)
            where TEntity : DbIdEntity<TKey>
    {
        var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType != null)
        {
            return entityType.GetNavigations().Where(p => !p.IsCollection).Select(x => x.Name).ToArray();
        }

        return Enumerable.Empty<string>();
    }
}
