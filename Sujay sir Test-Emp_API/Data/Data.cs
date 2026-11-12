using Microsoft.EntityFrameworkCore;
using Sujay_sir_Test_Emp_API.Models;

namespace Sujay_sir_Test_Emp_API.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Salary).HasPrecision(18, 2);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.Department).WithMany(x => x.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Department>(d =>
        {
            d.Property(x => x.Name).HasMaxLength(100).IsRequired();
            d.HasData(new Department { Id = 1, Name = "Engineering" }, new Department { Id = 2, Name = "Human Resources" });
        });
    }
}

