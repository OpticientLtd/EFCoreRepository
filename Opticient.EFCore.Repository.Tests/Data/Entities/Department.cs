
using Opticient.EFCore.Repository.Abstract.Entities;

namespace Opticient.EFCore.Repository.Tests.Data.Entities;

public class Department : IdEntity<int>
{
    public string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; } = [];
}
