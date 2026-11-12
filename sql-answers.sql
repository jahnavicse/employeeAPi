-- 1. Employee count and average salary by department; only groups with more than two employees.
SELECT d.Id, d.Name, COUNT(*) AS EmployeeCount, AVG(e.Salary) AS AverageSalary
FROM dbo.Departments AS d
JOIN dbo.Employees AS e ON e.DepartmentId = d.Id
WHERE e.IsActive = 1
GROUP BY d.Id, d.Name
HAVING COUNT(*) > 2;

-- 2. Employees earning above their department's average.
SELECT e.Id, e.Name, e.Email, e.Salary, e.DepartmentId
FROM dbo.Employees AS e
WHERE e.IsActive = 1
  AND e.Salary > (
      SELECT AVG(e2.Salary)
      FROM dbo.Employees AS e2
      WHERE e2.DepartmentId = e.DepartmentId AND e2.IsActive = 1
  );

-- 3. Second-highest distinct salary per department. Ties share the same rank.
WITH RankedSalaries AS (
    SELECT e.DepartmentId, e.Id, e.Name, e.Salary,
           DENSE_RANK() OVER (PARTITION BY e.DepartmentId ORDER BY e.Salary DESC) AS SalaryRank
    FROM dbo.Employees AS e
    WHERE e.IsActive = 1
)
SELECT DepartmentId, Id, Name, Salary
FROM RankedSalaries
WHERE SalaryRank = 2;

-- 4. A unique index on Email enforces uniqueness and supports equality lookups/searches
-- by email. The EF Core model configures this as a unique index.
select * from Departments
