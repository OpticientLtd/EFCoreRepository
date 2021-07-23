namespace Opticient.EFCore.Repository
{
    public interface IIdRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IDbEntity<TKey>
    {
    }
}
