using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Repositories;

namespace Opticient.EFCore.Repository.Tests;

[TestClass]
public class IdEntityReadRepositoryTests : UnitTestBase
{
    public IdEntityReadRepositoryTests()
    {
        EmployeeRepository = base.ServiceProvider.GetRequiredService<IEmployeeRepository>();
    }
    #region "G E T   A L L"

    [TestMethod]
    public async Task GetAll_NoFilterAndOrderByDescending_ShouldReturnRecords()
    {
        var records = await EmployeeRepository.GetAllAsync(true,
            orderBy: OrderByDescending);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count());
        records.First().Id.Should().Be(InitialTestData.InitialEmployees.Last().Id);
    }

    [TestMethod]
    public async Task GetAll_ValidFilter_ShouldReturnRecords()
    {
        var records = await EmployeeRepository.GetAllAsync(true,
            predicate: ValidPredicate,
            orderBy: OrderByDescending);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(ValidPredicate.Compile()));
        records.First().Id.Should().Be(InitialTestData.InitialEmployees.Last().Id);
    }

    [TestMethod]
    public async Task GetAll_InvalidFilter_ShouldNotReturnRecords()
    {
        var records = await EmployeeRepository.GetAllAsync(true,
            predicate: InvalidPredicate);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(InvalidPredicate.Compile()));
    }

    [TestMethod]
    public async Task GetAll_ValidFilterAndPage_ShouldReturnRecords()
    {
        var records = await EmployeeRepository.GetAllAsync(true,
            predicate: ValidPredicate,
            skipRecords: 1, returnRecords: 1);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Where(ValidPredicate.Compile()).Skip(1).Take(1).Count());
        records.First().Id.Should().Be(3);
    }

    [TestMethod]
    public async Task GetAll_ValidFilterAndWithDepartment_ShouldReturnRecords()
    {
        int id = InitialTestData.InitialEmployees.First().Id;
        var records = await EmployeeRepository.GetAllAsync(true,
            predicate: r => r.Id == id,
            includes: i => i.Include(s => s.Department));
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(r => r.Id == id));
        var first = records.First();
        first.Id.Should().Be(id);
        first.Department.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetAll_OrderByAndPage_ShouldReturnRecords()
    {
        var skipRecords = 1;
        var returnRecords = 1;
        var records = await EmployeeRepository.GetAllAsync(true,
            orderBy: OrderByDescending,
            skipRecords: skipRecords, returnRecords: returnRecords);
        var expectedRecords = OrderByDescending(InitialTestData.InitialEmployees.AsQueryable()).Skip(skipRecords).Take(returnRecords);
        records.Should().HaveCount(expectedRecords.Count());
        records.First().Id.Should().Be(expectedRecords.First().Id);
    }

    #endregion

    #region "G E T"

    [TestMethod]
    public async Task Get_ValidId_ShouldReturnRecord()
    {
        byte id = 2;
        var record = await EmployeeRepository.GetAsync(true, id);
        record.Should().NotBeNull();
        record.Id.Should().Be(id);
    }

    [TestMethod]
    public async Task Get_InvalidId_ShouldNotReturnRecord()
    {
        byte id = byte.MinValue;
        var record = await EmployeeRepository.GetAsync(true, id);
        record.Should().BeNull();
    }

    #endregion

    #region "G E T   W I T H   N A V I G A T I O N"

    [TestMethod]
    public async Task GetWithAllNavigations_ValidId_ShouldReturnRecord()
    {
        var record = await EmployeeRepository.GetWithAllNavigationsAsync(true, 1);
        record.Should().NotBeNull();
        record.Department.Should().NotBeNull();
        record.Department.Name.Should().Be(InitialTestData.InitialDepartments
            .First(d => d.Id == record.DepartmentId).Name);
    }

    #endregion

    #region "G E T   P R O J E C T E D"

    [TestMethod]
    public async Task GetProjected_ShouldReturnProjectedData()
    {
        var id = 1;
        var employee = InitialTestData.InitialEmployees.First(e => e.Id == id);
        var record = await EmployeeRepository.GetProjectedAsync(id, e => new { e.Id, e.Name, DepartmentName = e.Department.Name });
        record.Should().NotBeNull();
        record.Id.Should().Be(id);
        record.Name.Should().Be(employee.Name);
        record.DepartmentName.Should().Be(InitialTestData.InitialDepartments
            .First(d => d.Id == employee.DepartmentId).Name);
    }

    #endregion
}