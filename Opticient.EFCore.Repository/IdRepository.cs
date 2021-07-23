using Microsoft.EntityFrameworkCore;

namespace Opticient.EFCore.Repository
{
    public class IdRepository<TEntity, TKey, TContext> : Repository<TEntity, TKey, TContext>
        where TEntity : class, IDbEntity<TKey>
        where TContext : DbContext
    {
        public IdRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
