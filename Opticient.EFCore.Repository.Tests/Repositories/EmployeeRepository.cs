namespace Opticient.EFCore.Repository.Tests.Repositories;

using Opticient.EFCore.Repository.Abstract;
using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;

internal class EmployeeRepository : Repository<Employee, int>, IEmployeeRepository
{
    public EmployeeRepository(DemoDbContext dbContext) : base(dbContext)
    {
    }
}
