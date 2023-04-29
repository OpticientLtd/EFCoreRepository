namespace Opticient.EFCore.Repository.Tests;

using System.Linq;
using System.Linq.Expressions;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;
using Opticient.EFCore.Repository.Tests.Repositories;

[TestClass]
public class ReadRepositoryTests : UnitTestBase
{
    private readonly IEmployeeRepository _repository;
    private readonly Expression<Func<Employee, bool>> _validPredicate = e => e.Id > 1;
    private readonly Expression<Func<Employee, bool>> _invalidPredicate = e => e.Id > byte.MaxValue;
    private readonly Func<IQueryable<Employee>, IOrderedQueryable<Employee>> _orderByAscending = o => o.OrderBy(e => e.Id);
    private readonly Func<IQueryable<Employee>, IOrderedQueryable<Employee>> _orderByDescending = o => o.OrderByDescending(e => e.Id);


    public ReadRepositoryTests()
    {
        _repository = base.ServiceProvider.GetRequiredService<IEmployeeRepository>();
    }

    #region " A N Y"

    [TestMethod]
    public async Task Any_NoFilter_ShouldReturnTrue()
    {
        var result = await _repository.AnyAsync();
        result.Should().Be(InitialTestData.InitialEmployees.Any());
    }

    [TestMethod]
    public async Task Any_ValidFilter_ShouldReturnTrue()
    {
        var result = await _repository.AnyAsync(_validPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Any(_validPredicate.Compile()));
    }

    [TestMethod]
    public async Task Any_InvalidFilter_ShouldReturnFalse()
    {
        var result = await _repository.AnyAsync(_invalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Any(_invalidPredicate.Compile()));
    }

    #endregion

    #region "A L L"

    [TestMethod]
    public async Task All_InvalidFilter_ShouldReturnFalse()
    {
        var result = await _repository.AllAsync(_invalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.All(_invalidPredicate.Compile()));
    }

    [TestMethod]
    public async Task All_ValidFilter_ShouldReturnTrue()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id > 0;
        var result = await _repository.AllAsync(predicate);
        result.Should().Be(InitialTestData.InitialEmployees.All(predicate.Compile()));
    }

    #endregion

    #region "C O N T A I N S"

    [TestMethod]
    public async Task Contains_ValidEntity_ShouldReturnTrue()
    {
        var entity = await _repository.GetAsync(true, 1);
        var result = await _repository.ContainsAsync(entity);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task Contains_InvalidEntity_ShouldReturnFalse()
    {
        var entity = new Employee { Id = int.MaxValue };
        var result = await _repository.ContainsAsync(entity);
        result.Should().BeFalse();
    }

    #endregion

    #region "C O U N T"

    [TestMethod]
    public async Task Count_NoFilter_ShouldReturnRecords()
    {
        var result = await _repository.CountAsync();
        result.Should().Be(InitialTestData.InitialEmployees.Count());
    }

    [TestMethod]
    public async Task Count_ValidFilter_ShouldReturnRecords()
    {
        var result = await _repository.CountAsync(_validPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Count(_validPredicate.Compile()));
    }

    [TestMethod]
    public async Task Count_InvalidFilter_ShouldNotReturnRecords()
    {
        var result = await _repository.CountAsync(_invalidPredicate);
        result.Should().Be(InitialTestData.InitialEmployees.Count(_invalidPredicate.Compile()));
    }

    #endregion

    #region "F I R S T   OR   D E F A U L T"

    [TestMethod]
    public async Task FirstOrDefault_NoFilterAnd_orderByAscending_ShouldReturnFirstRecord()
    {
        var record = await _repository.FirstOrDefaultAsync(true, orderBy: _orderByAscending);
        record.Should().NotBeNull();
        record.Id.Should().Be(_orderByAscending(InitialTestData.InitialEmployees.AsQueryable()).FirstOrDefault().Id);
    }

    [TestMethod]
    public async Task FirstOrDefault_ValidFilter_ShouldReturnRecord()
    {
        var record = await _repository.FirstOrDefaultAsync(true,
            predicate: _validPredicate,
            orderBy: _orderByAscending);
        record.Should().NotBeNull();
        record.Id.Should().Be(_orderByAscending(InitialTestData.InitialEmployees
            .Where(_validPredicate.Compile()).AsQueryable()).FirstOrDefault().Id);
    }

    [TestMethod]
    public async Task FirstOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await _repository.FirstOrDefaultAsync(true,
            predicate: _invalidPredicate);
        record.Should().BeNull();
    }

    #endregion

    #region "L A S T   OR   D E F A U L T"

    [TestMethod]
    public async Task LastOrDefault_NoFilterAnd_orderByDescending_ShouldReturnLastRecord()
    {
        var record = await _repository.LastOrDefaultAsync(true, orderBy: _orderByDescending);
        record.Should().NotBeNull();
        record.Id.Should().Be(_orderByDescending(InitialTestData.InitialEmployees.AsQueryable()).LastOrDefault().Id);
    }

    [TestMethod]
    public async Task LastOrDefault_ValidFilter_ShouldReturnRecord()
    {
        var record = await _repository.LastOrDefaultAsync(true,
            predicate: _validPredicate,
            orderBy: _orderByDescending);
        record.Should().NotBeNull();
        record.Id.Should().Be(_orderByDescending(InitialTestData.InitialEmployees
            .Where(_validPredicate.Compile()).AsQueryable()).LastOrDefault().Id);
    }

    [TestMethod]
    public async Task LastOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await _repository.LastOrDefaultAsync(true,
            predicate: _invalidPredicate);
        record.Should().BeNull();
    }

    #endregion

    #region "S I N G L E   OR   D E F A U L T"

    [TestMethod]
    public async Task SingleOrDefault_NoFilter_ShouldReturnRecord()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id == 1;
        var record = await _repository.SingleOrDefaultAsync(true, predicate);
        record.Should().NotBeNull();
        record.Id.Should().Be(InitialTestData.InitialEmployees.SingleOrDefault(predicate.Compile()).Id);
    }

    [TestMethod]
    public async Task SingleOrDefault_ValidFilter_ShouldReturnRecord()
    {
        Expression<Func<Employee, bool>> predicate = d => d.Id == 1;
        var record = await _repository.SingleOrDefaultAsync(true, predicate);
        record.Should().NotBeNull();
        record.Id.Should().Be(InitialTestData.InitialEmployees.SingleOrDefault(predicate.Compile()).Id);
    }

    [TestMethod]
    public async Task SingleOrDefault_InvalidFilter_ShouldNotReturnRecord()
    {
        var record = await _repository.LastOrDefaultAsync(true,
            predicate: _invalidPredicate);
        record.Should().BeNull();
    }

    #endregion

    #region "G E T   A L L"

    [TestMethod]
    public async Task GetAll_NoFilterAnd_orderByDescending_ShouldReturnRecords()
    {
        var records = await _repository.GetAllAsync(true,
            orderBy: _orderByDescending);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count());
        records.First().Id.Should().Be(InitialTestData.InitialEmployees.Last().Id);
    }

    [TestMethod]
    public async Task GetAll_ValidFilter_ShouldReturnRecords()
    {
        var records = await _repository.GetAllAsync(true,
            predicate: _validPredicate,
            orderBy: _orderByDescending);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(_validPredicate.Compile()));
        records.First().Id.Should().Be(InitialTestData.InitialEmployees.Last().Id);
    }

    [TestMethod]
    public async Task GetAll_InvalidFilter_ShouldNotReturnRecords()
    {
        var records = await _repository.GetAllAsync(true,
            predicate: _invalidPredicate);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Count(_invalidPredicate.Compile()));
    }

    [TestMethod]
    public async Task GetAll_ValidFilterAndPage_ShouldReturnRecords()
    {
        var records = await _repository.GetAllAsync(true,
            predicate: _validPredicate,
            skipRecords: 1, returnRecords: 1);
        records.Should().HaveCount(InitialTestData.InitialEmployees.Where(_validPredicate.Compile()).Skip(1).Take(1).Count());
        records.First().Id.Should().Be(3);
    }

    [TestMethod]
    public async Task GetAll_ValidFilterAndWithDepartment_ShouldReturnRecords()
    {
        int id = InitialTestData.InitialEmployees.First().Id;
        var records = await _repository.GetAllAsync(true,
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
        var records = await _repository.GetAllAsync(true,
            orderBy: _orderByDescending,
            skipRecords: skipRecords, returnRecords: returnRecords);
        var expectedRecords = _orderByDescending(InitialTestData.InitialEmployees.AsQueryable()).Skip(skipRecords).Take(returnRecords);
        records.Should().HaveCount(expectedRecords.Count());
        records.First().Id.Should().Be(expectedRecords.First().Id);
    }

    #endregion

    #region "G E T"

    [TestMethod]
    public async Task Get_ValidId_ShouldReturnRecord()
    {
        byte id = 2;
        var record = await _repository.GetAsync(true, id);
        record.Should().NotBeNull();
        record.Id.Should().Be(id);
    }

    [TestMethod]
    public async Task Get_InvalidId_ShouldNotReturnRecord()
    {
        byte id = byte.MinValue;
        var record = await _repository.GetAsync(true, id);
        record.Should().BeNull();
    }

    #endregion

    #region "G E T   W I T H   N A V I G A T I O N"

    [TestMethod]
    public async Task GetWithAllNavigations_ValidId_ShouldReturnRecord()
    {
        var record = await _repository.GetWithAllNavigationsAsync(true, 1);
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
        var record = await _repository.GetProjectedAsync(id, e => new { e.Id, e.Name, DepartmentName = e.Department.Name });
        record.Should().NotBeNull();
        record.Id.Should().Be(id);
        record.Name.Should().Be(employee.Name);
        record.DepartmentName.Should().Be(InitialTestData.InitialDepartments
            .First(d => d.Id == employee.DepartmentId).Name);
    }

    #endregion

    #region "G E T   A L L   P R O J E C T E D"

    [TestMethod]
    public async Task GetAllProjected_ShouldReturnProjectedData()
    {
        Expression<Func<Employee, bool>> predicate = e => e.Id > 1;
        var employees = InitialTestData.InitialEmployees.Where(predicate.Compile())
            .Select(e => new { e.Id, e.Name }).ToArray();
        var records = (await _repository.GetAllProjectedAsync(e => new { e.Id, e.Name },
            predicate: predicate)).ToArray();
        records.Should().NotBeNull();
        records.Length.Should().Be(employees.Length);
        for (int i = 0; i < employees.Length; i++)
        {
            (employees[i].Id == records[i].Id && employees[i].Name == records[i].Name).Should().BeTrue();
        }
    }

    #endregion

    #region "S U M"

    [TestMethod]
    public async Task Sum_NoFilter_ShouldReturnRecords()
    {
        var result = await _repository.SumAsync(sumPredicate: r => r.Id);
        result.Should().Be(InitialTestData.InitialEmployees.Sum(r => r.Id));
    }

    [TestMethod]
    public async Task Sum_ValidFilter_ShouldReturnRecords()
    {
        var result = await _repository.SumAsync(filterPredicate: _validPredicate,
            sumPredicate: r => r.Id);
        result.Should().Be(InitialTestData.InitialEmployees.Where(_validPredicate.Compile()).Sum(r => r.Id));
    }

    #endregion

}