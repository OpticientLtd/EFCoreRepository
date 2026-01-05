using Opticient.EFCore.Repository.Interfaces.Repositories;
using Opticient.EFCore.Repository.Tests.Data.Entities;

namespace Opticient.EFCore.Repository.Tests.Repositories;

public interface IEmployeeRepository : IIdEntityRepository<Employee, int>
{
}
