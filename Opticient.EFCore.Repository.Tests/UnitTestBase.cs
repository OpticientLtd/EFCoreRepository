using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Opticient.EFCore.Repository.Tests.Data;
using Opticient.EFCore.Repository.Tests.Data.Entities;
using Opticient.EFCore.Repository.Tests.Repositories;

using System.Linq.Expressions;

namespace Opticient.EFCore.Repository.Tests;

public abstract class UnitTestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IServiceCollection Services { get; private set; }
    protected internal IEmployeeRepository EmployeeRepository { get; protected set; }
    protected internal IDepartmentRepository DepartmentRepository { get; protected set; }

    protected readonly Expression<Func<Employee, bool>> ValidPredicate = e => e.Id > 1;
    protected readonly Expression<Func<Employee, bool>> InvalidPredicate = e => e.Id > byte.MaxValue;
    protected readonly Func<IQueryable<Employee>, IOrderedQueryable<Employee>> OrderByAscending = o => o.OrderBy(e => e.Id);
    protected readonly Func<IQueryable<Employee>, IOrderedQueryable<Employee>> OrderByDescending = o => o.OrderByDescending(e => e.Id);

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
        Services.AddTransient(typeof(ILogger<>), typeof(NullLogger<>));
        ServiceProvider = Services.BuildServiceProvider();
        var dbContext = ServiceProvider.GetRequiredService<DemoDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
