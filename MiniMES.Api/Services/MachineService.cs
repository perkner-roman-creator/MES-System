using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;

    public MachineService(IMachineRepository machineRepository)
    {
        _machineRepository = machineRepository;
    }

    public async Task<IEnumerable<Machine>> GetAllMachinesAsync()
    {
        return await _machineRepository.GetAllAsync();
    }

    public async Task<Machine?> GetMachineByIdAsync(int id)
    {
        return await _machineRepository.GetByIdAsync(id);
    }

    public async Task<Machine> CreateMachineAsync(CreateMachineDto dto)
    {
        var existing = await _machineRepository.GetByMachineCodeAsync(dto.MachineCode);
        if (existing != null)
        {
            throw new InvalidOperationException($"Machine with code {dto.MachineCode} already exists.");
        }

        var machine = new Machine
        {
            MachineCode = dto.MachineCode,
            Name = dto.Name,
            Type = dto.Type,
            Location = dto.Location,
            Status = MachineStatus.Idle,
            EfficiencyRate = 100.0,
            Notes = dto.Notes
        };

        return await _machineRepository.CreateAsync(machine);
    }

    public async Task<Machine> UpdateMachineAsync(int id, UpdateMachineDto dto)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
        {
            throw new KeyNotFoundException($"Machine with ID {id} not found.");
        }

        machine.Name = dto.Name ?? machine.Name;
        machine.Type = dto.Type ?? machine.Type;
        machine.Location = dto.Location ?? machine.Location;
        machine.Notes = dto.Notes ?? machine.Notes;
        machine.EfficiencyRate = dto.EfficiencyRate ?? machine.EfficiencyRate;
        machine.LastMaintenanceDate = dto.LastMaintenanceDate ?? machine.LastMaintenanceDate;
        machine.NextMaintenanceDate = dto.NextMaintenanceDate ?? machine.NextMaintenanceDate;

        await _machineRepository.UpdateAsync(machine);
        return machine;
    }

    public async Task DeleteMachineAsync(int id)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
        {
            throw new KeyNotFoundException($"Machine with ID {id} not found.");
        }

        await _machineRepository.DeleteAsync(id);
    }

    public async Task<Machine> UpdateMachineStatusAsync(int id, MachineStatus status)
    {
        var machine = await _machineRepository.GetByIdAsync(id);
        if (machine == null)
        {
            throw new KeyNotFoundException($"Machine with ID {id} not found.");
        }

        machine.Status = status;
        await _machineRepository.UpdateAsync(machine);
        return machine;
    }

    public async Task<IEnumerable<Machine>> GetAvailableMachinesAsync()
    {
        return await _machineRepository.GetAvailableMachinesAsync();
    }
}
