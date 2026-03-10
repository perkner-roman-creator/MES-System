using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _employeeRepository.GetAllAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _employeeRepository.GetByIdAsync(id);
    }

    public async Task<Employee> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        // Generuj EmployeeCode pokud není zadán
        string employeeCode = dto.EmployeeCode;
        if (string.IsNullOrEmpty(employeeCode))
        {
            // Generuj sekvenciální kód: EMP-001, EMP-002, atd.
            var lastEmployee = (await _employeeRepository.GetAllAsync()).OrderBy(e => e.EmployeeCode).LastOrDefault();
            int nextNumber = 1;
            if (lastEmployee != null && lastEmployee.EmployeeCode.StartsWith("EMP-"))
            {
                if (int.TryParse(lastEmployee.EmployeeCode.Substring(4), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            employeeCode = $"EMP-{nextNumber:D3}";
        }
        
        var existing = await _employeeRepository.GetByEmployeeCodeAsync(employeeCode);
        if (existing != null)
        {
            throw new InvalidOperationException($"Employee with code {employeeCode} already exists.");
        }

        var employee = new Employee
        {
            EmployeeCode = employeeCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            Department = dto.Department,
            HiredDate = dto.HiredDate ?? DateTime.UtcNow,
            Skills = dto.Skills,
            Status = EmployeeStatus.Available
        };

        return await _employeeRepository.CreateAsync(employee);
    }

    public async Task<Employee> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        employee.FirstName = dto.FirstName ?? employee.FirstName;
        employee.LastName = dto.LastName ?? employee.LastName;
        employee.Email = dto.Email ?? employee.Email;
        employee.Phone = dto.Phone ?? employee.Phone;
        employee.Position = dto.Position ?? employee.Position;
        employee.Department = dto.Department ?? employee.Department;
        employee.Skills = dto.Skills ?? employee.Skills;

        await _employeeRepository.UpdateAsync(employee);
        return employee;
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        await _employeeRepository.DeleteAsync(id);
    }

    public async Task<Employee> UpdateEmployeeStatusAsync(int id, EmployeeStatus status)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        employee.Status = status;
        await _employeeRepository.UpdateAsync(employee);
        return employee;
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync()
    {
        return await _employeeRepository.GetAvailableEmployeesAsync();
    }
}
