using System.ComponentModel.DataAnnotations;

namespace Sujay_sir_Test_Emp_API.DTOs
{
    public class EmployeeDtos
    {
        public sealed record EmployeeCreateDto(
            [Required, StringLength(120)] string Name,
            [Required, EmailAddress, StringLength(256)] string Email,
            [Range(typeof(decimal), "0.01", "9999999999999999")] decimal Salary,
            DateOnly JoiningDate,
            [Range(1, int.MaxValue)] int DepartmentId);

        public sealed record EmployeeUpdateDto(
            [Required, StringLength(120)] string Name,
            [Required, EmailAddress, StringLength(256)] string Email,
            [Range(typeof(decimal), "0.01", "9999999999999999")] decimal Salary,
            DateOnly JoiningDate,
            [Range(1, int.MaxValue)] int DepartmentId);

        public sealed record EmployeeDto(int Id, string Name, string Email, decimal Salary, DateOnly JoiningDate, bool IsActive, int DepartmentId, string DepartmentName);

        public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

        public sealed record LoginRequest([Required] string Username, [Required] string Password);

        public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string Role);
    }
}