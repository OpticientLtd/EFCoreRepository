namespace Opticient.EFCore.Repository
{
    public interface IDbEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
