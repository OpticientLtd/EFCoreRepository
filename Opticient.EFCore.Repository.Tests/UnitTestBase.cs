namespace Opticient.EFCore.Repository.Tests;
using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Repositories;

public abstract class UnitTestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IServiceCollection Services { get; private set; }

    public UnitTestBase()
    {
        Services = new ServiceCollection();
        Action<DbContextOptionsBuilder> optionsAction = options =>
        {
            options.EnableSensitiveDataLogging(true)
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        };

        Services.AddDbContextPool<DemoDbContext>(optionsAction);
        Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        ServiceProvider = Services.BuildServiceProvider();
        var dbContext = ServiceProvider.GetRequiredService<DemoDbContext>();
        dbContext.Database.EnsureCreated();
    }

}
