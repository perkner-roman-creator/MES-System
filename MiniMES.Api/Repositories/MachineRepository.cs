using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public class MachineRepository : IMachineRepository
{
    private readonly MesDbContext _context;

    public MachineRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Machine>> GetAllAsync()
    {
        return await _context.Machines
            .Where(m => m.IsActive)
            .OrderBy(m => m.MachineCode)
            .ToListAsync();
    }

    public async Task<Machine?> GetByIdAsync(int id)
    {
        return await _context.Machines
            .Include(m => m.WorkOrders)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Machine?> GetByMachineCodeAsync(string machineCode)
    {
        return await _context.Machines
            .FirstOrDefaultAsync(m => m.MachineCode == machineCode);
    }

    public async Task<IEnumerable<Machine>> GetByStatusAsync(MachineStatus status)
    {
        return await _context.Machines
            .Where(m => m.Status == status && m.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Machine>> GetAvailableMachinesAsync()
    {
        return await _context.Machines
            .Where(m => m.Status == MachineStatus.Idle && m.IsActive)
            .ToListAsync();
    }

    public async Task<Machine> CreateAsync(Machine machine)
    {
        _context.Machines.Add(machine);
        await _context.SaveChangesAsync();
        return machine;
    }

    public async Task UpdateAsync(Machine machine)
    {
        _context.Entry(machine).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var machine = await _context.Machines.FindAsync(id);
        if (machine != null)
        {
            machine.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Machines.AnyAsync(m => m.Id == id);
    }
}
