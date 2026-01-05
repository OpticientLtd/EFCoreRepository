namespace Opticient.EFCore.Repository.Interfaces.Entities;

/// <summary>
/// Defines a base contract for entities that expose a unique identifier.
/// </summary>
/// <typeparam name="TKey">
/// The identifier type (e.g., int, Guid, string).
/// </typeparam>
public interface IIdEntity<TKey> : IEntityBase
{
    /// <summary>
    /// The primary identifier of the entity.
    /// </summary>
    TKey Id { get; set; }
}
