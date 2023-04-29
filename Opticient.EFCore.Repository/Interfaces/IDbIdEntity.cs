namespace Opticient.EFCore.Repository.Interfaces
{
    public interface IDbIdEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
