using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;
using Opticient.EFCore.Repository.Tests.Repositories;

namespace Opticient.EFCore.Repository.Tests;

[TestClass]
public class IdEntityRepositoryTests : UnitTestBase
{
    private readonly DemoDbContext _demoDbContext;
    public IdEntityRepositoryTests()
    {
        EmployeeRepository = base.ServiceProvider.GetRequiredService<IEmployeeRepository>();
        _demoDbContext = base.ServiceProvider.GetRequiredService<DemoDbContext>();
    }

    [TestMethod]
    public async Task AddUpdateDelete_ValidValues_ShouldbeAddedAndUpdatedAndDeleted()
    {
        int id = InitialTestData.InitialEmployees.Count() + 1;

        // Check doesn't exist
        var entity = await EmployeeRepository.GetAsync(true, id);
        entity.Should().BeNull();
        entity = new Data.Entities.Employee
        {
            Id = id,
            Name = "TestAdd",
            Salary = 1000,
            DepartmentId = InitialTestData.InitialDepartments.First().Id
        };

        // Add
        await EmployeeRepository.AddAsync(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);

        // Check added successfully
        entity = await EmployeeRepository.GetAsync(false, id);
        entity.Should().NotBeNull();
        entity.Id.Should().Be(id);

        // Update
        var updatedName = "Updated Name";
        entity.Name = updatedName;
        rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);

        // Check updated successfully
        entity = await EmployeeRepository.GetAsync(false, id);
        entity.Should().NotBeNull();
        entity.Name.Should().Be(updatedName);

        // Remove
        await EmployeeRepository.RemoveAsync(id);

        // Check removed successfully
        entity = await EmployeeRepository.GetAsync(false, id);
        entity.Should().NotBeNull();

    }

    #region "U P D A T E"

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
        EmployeeRepository.Update(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
    }

    #endregion "U P D A T E"

    #region "R E M O V E"

    [TestMethod]
    public async Task Remove_ValidEntity_ShouldbeDeleted()
    {
        var id = InitialTestData.InitialEmployees.Last().Id;
        var entity = await EmployeeRepository.GetAsync(false, id);
        EmployeeRepository.Remove(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
        entity = await EmployeeRepository.GetAsync(false, id);
        entity.Should().BeNull();
    }

    #endregion "R E M O V E"

    #region "A D D   R A N G E"

    [TestMethod]
    public async Task AddRange_ValidEntities_ShouldbeAAdded()
    {
        var id = InitialTestData.InitialEmployees.Last().Id;
        var entity = await EmployeeRepository.GetAsync(false, id);
        EmployeeRepository.Remove(entity);
        var rows = await _demoDbContext.SaveChangesAsync();
        rows.Should().Be(1);
        entity = await EmployeeRepository.GetAsync(false, id);
        entity.Should().BeNull();
    }

    #endregion "A D D   R A N G E"

    #region "I D   E X I S T S"

    [TestMethod]
    public async Task IdExists_ValidFilter_ShouldReturnTrue()
    {
        var result = await EmployeeRepository.IdExistsAsync(1);
        result.Should().Be(InitialTestData.InitialEmployees.Any(e => e.Id == 1));
    }

    #endregion
}