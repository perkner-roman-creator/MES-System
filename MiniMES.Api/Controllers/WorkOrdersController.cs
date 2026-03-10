using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.DTOs;
using MiniMES.Api.Models;
using MiniMES.Api.Services;

namespace MiniMES.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetAll()
    {
        var workOrders = await _workOrderService.GetAllWorkOrdersAsync();
        return Ok(workOrders);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<WorkOrder>>> GetActive()
    {
        var workOrders = await _workOrderService.GetActiveWorkOrdersAsync();
        return Ok(workOrders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkOrder>> GetById(int id)
    {
        var workOrder = await _workOrderService.GetWorkOrderByIdAsync(id);
        if (workOrder == null)
        {
            return NotFound();
        }
        return Ok(workOrder);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<ActionResult<WorkOrder>> Create([FromBody] CreateWorkOrderDto dto)
    {
        try
        {
            var workOrder = await _workOrderService.CreateWorkOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = workOrder.Id }, workOrder);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public async Task<ActionResult<WorkOrder>> Update(int id, [FromBody] UpdateWorkOrderDto dto)
    {
        try
        {
            var workOrder = await _workOrderService.UpdateWorkOrderAsync(id, dto);
            return Ok(workOrder);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _workOrderService.DeleteWorkOrderAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult<WorkOrder>> Start(int id, [FromQuery] int? machineId, [FromQuery] int? employeeId)
    {
        try
        {
            var workOrder = await _workOrderService.StartWorkOrderAsync(id, machineId, employeeId);
            return Ok(workOrder);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/pause")]
    public async Task<ActionResult<WorkOrder>> Pause(int id, [FromQuery] string? reason)
    {
        try
        {
            var workOrder = await _workOrderService.PauseWorkOrderAsync(id, reason);
            return Ok(workOrder);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<WorkOrder>> Complete(int id)
    {
        try
        {
            var workOrder = await _workOrderService.CompleteWorkOrderAsync(id);
            return Ok(workOrder);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/progress")]
    public async Task<ActionResult<WorkOrder>> UpdateProgress(
        int id, 
        [FromQuery] int quantityProduced, 
        [FromQuery] int quantityRejected = 0,
        [FromQuery] string? notes = null)
    {
        try
        {
            var workOrder = await _workOrderService.UpdateProductionProgressAsync(id, quantityProduced, quantityRejected, notes);
            return Ok(workOrder);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
