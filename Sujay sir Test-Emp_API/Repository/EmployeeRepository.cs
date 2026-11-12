using Microsoft.EntityFrameworkCore;
using Sujay_sir_Test_Emp_API.Data;
//using Sujay_sir_Test_Emp_API.DTOs;
using Sujay_sir_Test_Emp_API.Models;
using static Sujay_sir_Test_Emp_API.DTOs.EmployeeDtos;


namespace Sujay_sir_Test_Emp_API.Repository;

public sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    private static EmployeeDto ToDto(Employee e) => new(e.Id, e.Name, e.Email, e.Salary, e.JoiningDate, e.IsActive, e.DepartmentId, e.Department!.Name);

    public async Task<PagedResult<EmployeeDto>> GetPageAsync(string? search, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Employee> query = db.Employees.AsNoTracking().Include(e => e.Department).Where(e => e.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e => e.Name.Contains(term) || e.Email.Contains(term));
        }
        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(e => e.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(rows.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var e = await db.Employees.AsNoTracking().Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<EmployeeDto> AddAsync(EmployeeCreateDto dto, CancellationToken ct)
    {
        var e = new Employee { Name = dto.Name.Trim(), Email = dto.Email.Trim(), Salary = dto.Salary, JoiningDate = dto.JoiningDate, DepartmentId = dto.DepartmentId };
        db.Employees.Add(e);
        await db.SaveChangesAsync(ct);
        await db.Entry(e).Reference(x => x.Department).LoadAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct)
    {
        var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (e is null) return false;
        e.Name = dto.Name.Trim(); e.Email = dto.Email.Trim(); e.Salary = dto.Salary; e.JoiningDate = dto.JoiningDate; e.DepartmentId = dto.DepartmentId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken ct)
    {
        var e = await db.Employees.FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (e is null) return false;
        e.IsActive = false;
        await db.SaveChangesAsync(ct);
        return true;
    }
    public Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct) =>
    db.Departments.AnyAsync(d => d.Id == departmentId, ct);
}

