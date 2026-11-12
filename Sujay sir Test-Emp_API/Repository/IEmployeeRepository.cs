using static Sujay_sir_Test_Emp_API.DTOs.EmployeeDtos;

namespace Sujay_sir_Test_Emp_API.Repository;

public interface IEmployeeRepository
{
    Task<PagedResult<EmployeeDto>> GetPageAsync(string? search, int page, int pageSize, CancellationToken ct);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<EmployeeDto> AddAsync(EmployeeCreateDto dto, CancellationToken ct);
    Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct);
    Task<bool> SoftDeleteAsync(int id, CancellationToken ct);
    Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken ct);

}
