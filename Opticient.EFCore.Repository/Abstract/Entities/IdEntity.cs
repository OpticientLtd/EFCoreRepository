using Opticient.EFCore.Repository.Interfaces.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opticient.EFCore.Repository.Abstract.Entities;

/// <inheritdoc />
public abstract class IdEntity<TKey>
    : EntityBase, IIdEntity<TKey>
{
    [Required]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual TKey Id { get; set; }
}
