namespace Opticient.EFCore.Repository.Tests.Repositories;

using Opticient.EFCore.Repository.Abstract;
using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;

internal class DepartmentRepository : Repository<Department, int>, IDepartmentRepository
{
    public DepartmentRepository(DemoDbContext dbContext) : base(dbContext)
    {
    }
}
