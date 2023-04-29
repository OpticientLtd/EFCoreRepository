namespace Opticient.EFCore.Repository.Tests.Data.Entities;

using Opticient.EFCore.Repository.Abstract;

internal class Employee : DbIdEntity<int>
{
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public int Salary { get; set; }
    public virtual Department Department { get; set; }
}
