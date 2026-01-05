using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Abstract.Repositories;
using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;

namespace Opticient.EFCore.Repository.Tests.Repositories;

public class DepartmentRepository : IdEntityRepository<Department, int>, IDepartmentRepository
{
    public DepartmentRepository(DemoDbContext dbContext, ILogger<DepartmentRepository> logger) : base(dbContext, logger)
    {
    }
}
