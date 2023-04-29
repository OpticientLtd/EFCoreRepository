namespace Opticient.EFCore.Repository.Tests.Repositories;

using Opticient.EFCore.Repository.Interfaces;
using Opticient.EFCore.Repository.Tests.Data.Entities;

internal interface IDepartmentRepository : IRepository<Department, int>
{
}
