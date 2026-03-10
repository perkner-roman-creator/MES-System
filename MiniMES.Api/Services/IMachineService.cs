using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IMachineService
{
    Task<IEnumerable<Machine>> GetAllMachinesAsync();
    Task<Machine?> GetMachineByIdAsync(int id);
    Task<Machine> CreateMachineAsync(CreateMachineDto dto);
    Task<Machine> UpdateMachineAsync(int id, UpdateMachineDto dto);
    Task DeleteMachineAsync(int id);
    Task<Machine> UpdateMachineStatusAsync(int id, MachineStatus status);
    Task<IEnumerable<Machine>> GetAvailableMachinesAsync();
}
