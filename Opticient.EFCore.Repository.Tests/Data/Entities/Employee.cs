
using Opticient.EFCore.Repository.Abstract.Entities;

namespace Opticient.EFCore.Repository.Tests.Data.Entities;

public class Employee : IdEntity<int>
{
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public int Salary { get; set; }
    public virtual Department Department { get; set; }
}
