using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.DTOs;
using MiniMES.Api.Services;

namespace MiniMES.Api.Controllers;

/// <summary>
/// API controller for managing production operations (operace výroby)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OperationsController : ControllerBase
{
    private readonly IOperationService _operationService;

    public OperationsController(IOperationService operationService)
    {
        _operationService = operationService;
    }

    /// <summary>
    /// Get all production operations
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetAllOperations()
    {
        var operations = await _operationService.GetAllOperationsAsync();
        return Ok(operations);
    }

    /// <summary>
    /// Get operation by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OperationDto>> GetOperation(int id)
    {
        var operation = await _operationService.GetOperationByIdAsync(id);
        if (operation == null)
            return NotFound(new { message = $"Operation with ID {id} not found" });
        
        return Ok(operation);
    }

    /// <summary>
    /// Get operation by code
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<OperationDto>> GetOperationByCode(string code)
    {
        var operation = await _operationService.GetOperationByCodeAsync(code);
        if (operation == null)
            return NotFound(new { message = $"Operation with code {code} not found" });
        
        return Ok(operation);
    }

    /// <summary>
    /// Get all quality check points
    /// </summary>
    [HttpGet("quality-check-points")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetQualityCheckPoints()
    {
        var operations = await _operationService.GetQualityCheckPointsAsync();
        return Ok(operations);
    }

    /// <summary>
    /// Create new operation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OperationDto>> CreateOperation(CreateOperationDto dto)
    {
        try
        {
            var operation = await _operationService.CreateOperationAsync(dto);
            return CreatedAtAction(nameof(GetOperation), new { id = operation.Id }, operation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update operation
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<OperationDto>> UpdateOperation(int id, UpdateOperationDto dto)
    {
        try
        {
            var operation = await _operationService.UpdateOperationAsync(id, dto);
            return Ok(operation);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete operation
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOperation(int id)
    {
        try
        {
            await _operationService.DeleteOperationAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get operations for a specific work order
    /// </summary>
    [HttpGet("work-order/{workOrderId}")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetWorkOrderOperations(int workOrderId)
    {
        var operations = await _operationService.GetOperationsByWorkOrderAsync(workOrderId);
        return Ok(operations);
    }

    /// <summary>
    /// Add operation to work order (define routing)
    /// </summary>
    [HttpPost("work-order/{workOrderId}")]
    public async Task<ActionResult<WorkOrderOperationDto>> AddOperationToWorkOrder(int workOrderId, CreateWorkOrderOperationDto dto)
    {
        try
        {
            var workOrderOperation = await _operationService.AddOperationToWorkOrderAsync(workOrderId, dto);
            return CreatedAtAction(nameof(GetOperation), new { id = workOrderOperation.Id }, workOrderOperation);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Log operation event (start, progress, complete, etc.)
    /// </summary>
    [HttpPost("log")]
    public async Task<ActionResult<OperationLogDto>> LogOperationEvent(CreateOperationLogDto dto)
    {
        try
        {
            var log = await _operationService.LogOperationEventAsync(dto);
            return CreatedAtAction(nameof(GetOperation), new { id = log.Id }, log);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
