namespace Opticient.EFCore.Repository.Tests.Data;

using Microsoft.EntityFrameworkCore;

using Opticient.EFCore.Repository.Tests.Data.Entities;

internal class DemoDbContext : DbContext
{
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Department> Departments { get; set; }

    public DemoDbContext(DbContextOptions<DemoDbContext> options)
            : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasData(InitialTestData.InitialDepartments);
        });

        modelBuilder.Entity<Employee>(entity =>
        {

            entity.HasOne(d => d.Department)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(InitialTestData.InitialEmployees);
        });
    }
}
