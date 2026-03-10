using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<Employee> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
    Task DeleteEmployeeAsync(int id);
    Task<Employee> UpdateEmployeeStatusAsync(int id, EmployeeStatus status);
    Task<IEnumerable<Employee>> GetAvailableEmployeesAsync();
}
