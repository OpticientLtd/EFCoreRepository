namespace Opticient.EFCore.Repository.Tests.Data.Entities;

using Opticient.EFCore.Repository.Abstract;

internal class Department : DbIdEntity<int>
{
    public string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
