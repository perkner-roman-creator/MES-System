using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public interface IMachineRepository
{
    Task<IEnumerable<Machine>> GetAllAsync();
    Task<Machine?> GetByIdAsync(int id);
    Task<Machine?> GetByMachineCodeAsync(string machineCode);
    Task<IEnumerable<Machine>> GetByStatusAsync(MachineStatus status);
    Task<IEnumerable<Machine>> GetAvailableMachinesAsync();
    Task<Machine> CreateAsync(Machine machine);
    Task UpdateAsync(Machine machine);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
