namespace Opticient.EFCore.Repository.Tests.Data;
using System.Collections.Generic;

using Opticient.EFCore.Repository.Tests.Data.Entities;

internal static class InitialTestData
{
    public static IEnumerable<Department> InitialDepartments => new[]
                {
                    new Department{ Id = 1, Name = "Computer"},
                    new Department{ Id = 2, Name = "Account"},
                    new Department{ Id = 3, Name = "Engineering"},
                    new Department{ Id = 4, Name = "Human Resource"}
                };

    public static IEnumerable<Employee> InitialEmployees => new[] {
            new Employee {Id = 1,Name="John",DepartmentId=3,Salary=25000},
            new Employee {Id = 2,Name="Robert",DepartmentId=3,Salary=15000},
            new Employee {Id = 3,Name="Richard",DepartmentId=2,Salary=10000},
            new Employee {Id = 4,Name="Mark",DepartmentId=2,Salary=7500},
            new Employee {Id = 5,Name="Stefan",DepartmentId=1,Salary=5000},
        };
}
