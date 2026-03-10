using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.DTOs;
using MiniMES.Api.Models;
using MiniMES.Api.Services;

namespace MiniMES.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MachinesController : ControllerBase
{
    private readonly IMachineService _machineService;

    public MachinesController(IMachineService machineService)
    {
        _machineService = machineService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Machine>>> GetAll()
    {
        var machines = await _machineService.GetAllMachinesAsync();
        return Ok(machines);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Machine>>> GetAvailable()
    {
        var machines = await _machineService.GetAvailableMachinesAsync();
        return Ok(machines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Machine>> GetById(int id)
    {
        var machine = await _machineService.GetMachineByIdAsync(id);
        if (machine == null)
        {
            return NotFound();
        }
        return Ok(machine);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<ActionResult<Machine>> Create([FromBody] CreateMachineDto dto)
    {
        try
        {
            var machine = await _machineService.CreateMachineAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = machine.Id }, machine);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<ActionResult<Machine>> Update(int id, [FromBody] UpdateMachineDto dto)
    {
        try
        {
            var machine = await _machineService.UpdateMachineAsync(id, dto);
            return Ok(machine);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _machineService.DeleteMachineAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<Machine>> UpdateStatus(int id, [FromQuery] MachineStatus status)
    {
        try
        {
            var machine = await _machineService.UpdateMachineStatusAsync(id, status);
            return Ok(machine);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
