
using Microsoft.Extensions.Logging;

using Opticient.EFCore.Repository.Abstract.Repositories;
using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;

namespace Opticient.EFCore.Repository.Tests.Repositories;

public class EmployeeRepository : IdEntityRepository<Employee, int>, IEmployeeRepository
{
    public EmployeeRepository(DemoDbContext dbContext, ILogger<EmployeeRepository> logger) : base(dbContext, logger)
    {
    }
}
