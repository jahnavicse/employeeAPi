namespace Sujay_sir_Test_Emp_API.Models;

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public decimal Salary { get; set; }
    public DateOnly JoiningDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

}
