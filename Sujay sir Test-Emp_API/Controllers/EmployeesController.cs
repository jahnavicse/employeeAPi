using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sujay_sir_Test_Emp_API.Data;
using Sujay_sir_Test_Emp_API.Repository;
using static Sujay_sir_Test_Emp_API.DTOs.EmployeeDtos;



namespace Sujay_sir_Test_Emp_API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EmployeesController(IEmployeeRepository employees) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100) return BadRequest("page must be >= 1 and pageSize must be between 1 and 100.");
        return Ok(await employees.GetPageAsync(search, page, pageSize, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(id, ct);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto dto, CancellationToken ct)
    {
        // in Create and Update, replace the db.Departments line with:
        if (!await employees.DepartmentExistsAsync(dto.DepartmentId, ct))
            return BadRequest("DepartmentId does not exist.");
        try
        {
            var created = await employees.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DbUpdateException) { return Conflict("An employee with this email may already exist."); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, EmployeeUpdateDto dto, CancellationToken ct)
    {
        // in Create and Update, replace the db.Departments line with:
        if (!await employees.DepartmentExistsAsync(dto.DepartmentId, ct))
            return BadRequest("DepartmentId does not exist.");
        try { return await employees.UpdateAsync(id, dto, ct) ? NoContent() : NotFound(); }
        catch (DbUpdateException) { return Conflict("An employee with this email may already exist."); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) => await employees.SoftDeleteAsync(id, ct) ? NoContent() : NotFound();
}
