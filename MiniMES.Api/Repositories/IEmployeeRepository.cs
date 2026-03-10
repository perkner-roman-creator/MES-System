using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmployeeCodeAsync(string employeeCode);
    Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status);
    Task<IEnumerable<Employee>> GetAvailableEmployeesAsync();
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
