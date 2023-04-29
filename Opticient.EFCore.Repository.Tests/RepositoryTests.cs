namespace Opticient.EFCore.Repository.Tests;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;
using Opticient.EFCore.Repository.Tests.Repositories;

[TestClass]
public class RepositoryTests : UnitTestBase
{
    private readonly IEmployeeRepository _repository;
    private readonly DemoDbContext _demoDbContext;
    public RepositoryTests()
    {
        _repository = base.ServiceProvider.GetRequiredService<IEmployeeRepository>();
        _demoDbContext = base.ServiceProvider.GetRequiredService<DemoDbContext>();
    }

    [TestMethod]
    public async Task AddUpdateDelete_ValidValues_ShouldbeAddedAndUpdatedAndDeleted()
    {
        int id = InitialTestData.InitialEmployees.Count() + 1;

        // Check doesn't exist
        var entity = await _repository.GetAsync(true, id);
        entity.Should().BeNull();
        entity = new Data.Entities.Employee
        {
            Id = id,
            Name = "TestAdd",
            Salary = 1000,
            DepartmentId = InitialTestData.InitialDepartments.First().Id
        };

        // Add
        await _repository.AddAsync(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);

        // Check added successfully
        entity = await _repository.GetAsync(false, id);
        entity.Should().NotBeNull();
        entity.Id.Should().Be(id);

        // Update
        var updatedName = "Updated Name";
        entity.Name = updatedName;
        rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);

        // Check updated successfully
        entity = await _repository.GetAsync(false, id);
        entity.Should().NotBeNull();
        entity.Name.Should().Be(updatedName);

        // Remove
        await _repository.RemoveAsync(id);

        // Check removed successfully
        entity = await _repository.GetAsync(false, id);
        entity.Should().NotBeNull();

    }

    [TestMethod]
    public async Task Update_ValidValue_ShouldbeUpdated()
    {
        var updatedName = "Updated Name";
        var employee = InitialTestData.InitialEmployees.First();
        var entity = new Employee
        {
            Id = employee.Id,
            Name = updatedName,
            DepartmentId = employee.DepartmentId,
            Salary = employee.Salary
        };
        _repository.Update(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
    }

    [TestMethod]
    public async Task Remove_ValidEntity_ShouldbeDeleted()
    {
        var id = InitialTestData.InitialEmployees.Last().Id;
        var entity = await _repository.GetAsync(false, id);
        _repository.Remove(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
        entity = await _repository.GetAsync(false, id);
        entity.Should().BeNull();
    }

    [TestMethod]
    public async Task AddRange_ValidEntities_ShouldbeAAdded()
    {
        var id = InitialTestData.InitialEmployees.Last().Id;
        var entity = await _repository.GetAsync(false, id);
        _repository.Remove(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
        entity = await _repository.GetAsync(false, id);
        entity.Should().BeNull();
    }
}
