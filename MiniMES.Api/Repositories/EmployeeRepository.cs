using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly MesDbContext _context;

    public EmployeeRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.EmployeeCode)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.WorkOrders)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByEmployeeCodeAsync(string employeeCode)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
    }

    public async Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status)
    {
        return await _context.Employees
            .Where(e => e.Status == status && e.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync()
    {
        return await _context.Employees
            .Where(e => e.Status == EmployeeStatus.Available && e.IsActive)
            .ToListAsync();
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            employee.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }
}
