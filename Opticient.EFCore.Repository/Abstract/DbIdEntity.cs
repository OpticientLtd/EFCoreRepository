namespace Opticient.EFCore.Repository.Abstract;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Opticient.EFCore.Repository.Interfaces;

public abstract class DbIdEntity<TKey>
    : IDbIdEntity<TKey>
{
    [Required]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual TKey Id { get; set; }
}
