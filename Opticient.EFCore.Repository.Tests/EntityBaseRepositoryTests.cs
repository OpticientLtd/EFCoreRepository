using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;
using Opticient.EFCore.Repository.Tests.Repositories;

using System.Linq.Expressions;

namespace Opticient.EFCore.Repository.Tests;

[TestClass]
public class EntityBaseRepositoryTests : UnitTestBase
{

    public EntityBaseRepositoryTests()
    {
        EmployeeRepository = base.ServiceProvider.GetRequiredService<IEmployeeRepository>();
    }

    #region " A N Y"

    [TestMethod]
    public async Task Any_NoFilter_ShouldReturnTrue()
    {
        var result = await EmployeeRepository.AnyAsync();
        result.Should().Be(InitialTestData.InitialEmployees.Any());
    }

    [TestMethod]
    public async Task Any_ValidFilter_ShouldReturnTrue()
    {
        var result = await EmployeeRepository.AnyAsync(ValidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Any(ValidPredicate.Compile()));
    }

    [TestMethod]
    public async Task Any_InvalidFilter_ShouldReturnFalse()
    {
        var result = await EmployeeRepository.AnyAsync(InvalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Any(InvalidPredicate.Compile()));
    }

    #endregion

    #region "A L L"

    [TestMethod]
    public async Task All_InvalidFilter_ShouldReturnFalse()
    {
        var result = await EmployeeRepository.AllAsync(InvalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.All(InvalidPredicate.Compile()));
    }

    [TestMethod]
    public async Task All_ValidFilter_ShouldReturnTrue()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id > 0;
        var result = await EmployeeRepository.AllAsync(predicate);
        result.Should().Be(InitialTestData.InitialEmployees.All(predicate.Compile()));
    }

    #endregion

    #region "C O N T A I N S"

    [TestMethod]
    public async Task Contains_ValidEntity_ShouldReturnTrue()
    {
        var entity = await EmployeeRepository.GetAsync(true, 1);
        var result = await EmployeeRepository.ContainsAsync(entity);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task Contains_InvalidEntity_ShouldReturnFalse()
    {
        var entity = new Employee { Id = int.MaxValue };
        var result = await EmployeeRepository.ContainsAsync(entity);
        result.Should().BeFalse();
    }

    #endregion

    #region "C O U N T"

    [TestMethod]
    public async Task Count_NoFilter_ShouldReturnRecords()
    {
        var result = await EmployeeRepository.CountAsync();
        result.Should().Be(InitialTestData.InitialEmployees.Count());
    }

    [TestMethod]
    public async Task Count_ValidFilter_ShouldReturnRecords()
    {
        var result = await EmployeeRepository.CountAsync(ValidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Count(ValidPredicate.Compile()));
    }

    [TestMethod]
    public async Task Count_InvalidFilter_ShouldNotReturnRecords()
    {
        var result = await EmployeeRepository.CountAsync(InvalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Count(InvalidPredicate.Compile()));
    }

    #endregion

    #region "F I R S T   OR   D E F A U L T"

    [TestMethod]
    public async Task FirstOrDefault_NoFilterAndOrderByAscending_ShouldReturnFirstRecord()
    {
        var record = await EmployeeRepository.FirstOrDefaultAsync(true, orderBy: OrderByAscending);
        record.Should().NotBeNull();
        record.Id.Should().Be(OrderByAscending(InitialTestData.InitialEmployees.AsQueryable()).FirstOrDefault().Id);
    }

    [TestMethod]
    public async Task FirstOrDefault_ValidFilter_ShouldReturnRecord()
    {
        var record = await EmployeeRepository.FirstOrDefaultAsync(true,
            predicate: ValidPredicate,
            orderBy: OrderByAscending);
        record.Should().NotBeNull();
        record.Id.Should().Be(OrderByAscending(InitialTestData.InitialEmployees
            .Where(ValidPredicate.Compile()).AsQueryable()).FirstOrDefault().Id);
    }

    [TestMethod]
    public async Task FirstOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await EmployeeRepository.FirstOrDefaultAsync(true,
            predicate: InvalidPredicate);
        record.Should().BeNull();
    }

    #endregion

    #region "L A S T   OR   D E F A U L T"

    [TestMethod]
    public async Task LastOrDefault_NoFilterAndOrderByDescending_ShouldReturnLastRecord()
    {
        var record = await EmployeeRepository.LastOrDefaultAsync(true, orderBy: OrderByDescending);
        record.Should().NotBeNull();
        record.Id.Should().Be(OrderByDescending(InitialTestData.InitialEmployees.AsQueryable()).LastOrDefault().Id);
    }

    [TestMethod]
    public async Task LastOrDefault_ValidFilter_ShouldReturnRecord()
    {
        var record = await EmployeeRepository.LastOrDefaultAsync(true,
            predicate: ValidPredicate,
            orderBy: OrderByDescending);
        record.Should().NotBeNull();
        record.Id.Should().Be(OrderByDescending(InitialTestData.InitialEmployees
            .Where(ValidPredicate.Compile()).AsQueryable()).LastOrDefault().Id);
    }

    [TestMethod]
    public async Task LastOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await EmployeeRepository.LastOrDefaultAsync(true,
            predicate: InvalidPredicate);
        record.Should().BeNull();
    }

    #endregion

    #region "S I N G L E   OR   D E F A U L T"

    [TestMethod]
    public async Task SingleOrDefault_NoFilter_ShouldReturnRecord()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id == 1;
        var record = await EmployeeRepository.SingleOrDefaultAsync(true, predicate);
        record.Should().NotBeNull();
        record.Id.Should().Be(InitialTestData.InitialEmployees.SingleOrDefault(predicate.Compile()).Id);
    }

    [TestMethod]
    public async Task SingleOrDefault_ValidFilter_ShouldReturnRecord()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id == 1;
        var record = await EmployeeRepository.SingleOrDefaultAsync(true, predicate);
        record.Should().NotBeNull();
        record.Id.Should().Be(InitialTestData.InitialEmployees.SingleOrDefault(predicate.Compile()).Id);
    }

    [TestMethod]
    public async Task SingleOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await EmployeeRepository.LastOrDefaultAsync(true,
            predicate: InvalidPredicate);
        record.Should().BeNull();
    }

    #endregion

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

    #region "G E T   A L L   W I T H   N A V I G A T I O N"

    [TestMethod]
    public async Task GetAllWithAllNavigations_ValidId_ShouldReturnRecord()
    {
        var records = await EmployeeRepository.GetAllWithAllNavigationsAsync(true, predicate: ValidPredicate);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(ValidPredicate.Compile()));
        foreach (var record in records)
        {
            record.Department.Should().NotBeNull();
            record.Department.Name.Should().Be(InitialTestData.InitialDepartments
                .First(d => d.Id == record.DepartmentId).Name);
        }
    }

    #endregion

    #region "G E T   A L L   P R O J E C T E D"

    [TestMethod]
    public async Task GetAllProjected_EntityOrderBy_ShouldReturnProjectedData()
    {
        Expression<Func<Employee, bool>> predicate = e => e.Id > 1;
        var employees = InitialTestData.InitialEmployees.Where(predicate.Compile())
            .Select(e => new { e.Id, e.Name })
            .OrderBy(e => e.Name)
            .ToArray();
        var records = (await EmployeeRepository.GetAllProjectedAsync(e => new { e.Id, e.Name },
            predicate: predicate, entityOrderBy: o => o.OrderBy(e => e.Name))).ToArray();
        records.Should().NotBeNull();
        records.Length.Should().Be(employees.Length);
        records.Should().BeInAscendingOrder(e => e.Name);
        records.Should().Equal(employees);
    }

    [TestMethod]
    public async Task GetAllProjected_ProjectedOrderBy_ShouldReturnProjectedData()
    {
        Expression<Func<Employee, bool>> predicate = e => e.Id > 1;
        var employees = InitialTestData.InitialEmployees.Where(predicate.Compile())
            .Select(e => new { e.Name, e.Salary, e.Id })
            .OrderBy(e => e.Salary).ThenBy(e => e.Name).ThenBy(e => e.Id)
            .ToArray();
        var records = (await EmployeeRepository.GetAllProjectedAsync(e => new { e.Name, e.Salary, e.Id },
            predicate: predicate, projectedTypeOrderBy: o => o.OrderBy(e => e.Salary).ThenBy(e => e.Name).ThenBy(e => e.Id))).ToArray();
        records.Should().NotBeNull();
        records.Length.Should().Be(employees.Length);
        records.Should().Equal(employees);
    }

    #endregion

    #region "G E T   A L L   D I S T I N C T   P R O J E C T E D"

    [TestMethod]
    public async Task GetAllDistinctProjected_ShouldReturnProjectedData()
    {
        Expression<Func<Employee, bool>> predicate = e => e.Id > 1;
        var departments = InitialTestData.InitialEmployees.Where(predicate.Compile())
            .Join(InitialTestData.InitialDepartments,
                emp => emp.DepartmentId,
                dept => dept.Id,
                (emp, dept) => new { dept.Name })
            .Distinct()
            .OrderBy(e => e.Name)
            .ToArray();
        var records = (await EmployeeRepository.GetAllDistinctProjectedAsync(e => new { e.Department.Name },
            predicate: predicate, projectedTypeOrderBy: o => o.OrderBy(e => e.Name))).ToArray();
        records.Should().NotBeNull();
        records.Length.Should().Be(departments.Length);
        records.Should().BeInAscendingOrder(e => e.Name);
        records.Should().Equal(departments);
    }

    #endregion

    #region "S U M"

    [TestMethod]
    public async Task Sum_NoFilter_ShouldReturnRecords()
    {
        var result = await EmployeeRepository.SumAsync(sumPredicate: r => r.Id);
        result.Should().Be(InitialTestData.InitialEmployees.Sum(r => r.Id));
    }

    [TestMethod]
    public async Task Sum_ValidFilter_ShouldReturnRecords()
    {
        var result = await EmployeeRepository.SumAsync(filterPredicate: ValidPredicate,
            sumPredicate: r => r.Id);
        result.Should().Be(InitialTestData.InitialEmployees.Where(ValidPredicate.Compile()).Sum(r => r.Id));
    }

    #endregion
}
