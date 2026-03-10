using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.DTOs;
using MiniMES.Api.Services;

namespace MiniMES.Api.Controllers;

/// <summary>
/// API controller for managing production batches and serial numbers (dávky a sériová čísla)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;

    public BatchesController(IBatchService batchService)
    {
        _batchService = batchService;
    }

    /// <summary>
    /// Get all production batches
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BatchDto>>> GetAllBatches()
    {
        var batches = await _batchService.GetAllBatchesAsync();
        return Ok(batches);
    }

    /// <summary>
    /// Get batch by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BatchDto>> GetBatch(int id)
    {
        var batch = await _batchService.GetBatchByIdAsync(id);
        if (batch == null)
            return NotFound(new { message = $"Batch with ID {id} not found" });
        
        return Ok(batch);
    }

    /// <summary>
    /// Get batch by batch number
    /// </summary>
    [HttpGet("number/{batchNumber}")]
    public async Task<ActionResult<BatchDto>> GetBatchByNumber(string batchNumber)
    {
        var batch = await _batchService.GetBatchByNumberAsync(batchNumber);
        if (batch == null)
            return NotFound(new { message = $"Batch with number {batchNumber} not found" });
        
        return Ok(batch);
    }

    /// <summary>
    /// Get batches for a specific work order
    /// </summary>
    [HttpGet("work-order/{workOrderId}")]
    public async Task<ActionResult<IEnumerable<BatchDto>>> GetBatchesByWorkOrder(int workOrderId)
    {
        var batches = await _batchService.GetBatchesByWorkOrderAsync(workOrderId);
        return Ok(batches);
    }

    /// <summary>
    /// Get all batches pending quality check
    /// </summary>
    [HttpGet("pending-qc")]
    public async Task<ActionResult<IEnumerable<BatchDto>>> GetPendingQcBatches()
    {
        var batches = await _batchService.GetPendingQcBatchesAsync();
        return Ok(batches);
    }

    /// <summary>
    /// Create new batch
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BatchDto>> CreateBatch(CreateBatchDto dto)
    {
        try
        {
            var batch = await _batchService.CreateBatchAsync(dto);
            return CreatedAtAction(nameof(GetBatch), new { id = batch.Id }, batch);
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
    /// Update batch information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BatchDto>> UpdateBatch(int id, UpdateBatchDto dto)
    {
        try
        {
            var batch = await _batchService.UpdateBatchAsync(id, dto);
            return Ok(batch);
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
    /// Start batch production
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult<BatchDto>> StartBatch(int id, [FromQuery] int employeeId)
    {
        try
        {
            var batch = await _batchService.StartBatchAsync(id, employeeId);
            return Ok(batch);
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
    /// Send batch to quality control
    /// </summary>
    [HttpPost("{id}/send-to-qc")]
    public async Task<ActionResult<BatchDto>> SendToQc(int id)
    {
        try
        {
            var batch = await _batchService.SendToQcAsync(id);
            return Ok(batch);
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
    /// Complete batch (approve)
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<BatchDto>> CompleteBatch(int id)
    {
        try
        {
            var batch = await _batchService.CompleteBatchAsync(id);
            return Ok(batch);
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
    /// Delete batch
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBatch(int id)
    {
        try
        {
            await _batchService.DeleteBatchAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all serial numbers for a batch
    /// </summary>
    [HttpGet("{batchId}/serials")]
    public async Task<ActionResult<IEnumerable<SerialNumberDto>>> GetBatchSerials(int batchId)
    {
        var serials = await _batchService.GetSerialNumbersByBatchAsync(batchId);
        return Ok(serials);
    }

    /// <summary>
    /// Create serial numbers for a batch
    /// </summary>
    [HttpPost("{batchId}/serials")]
    public async Task<ActionResult<SerialNumberDto>> CreateSerialNumber(int batchId, CreateSerialNumberDto dto)
    {
        try
        {
            dto.BatchId = batchId;
            var serial = await _batchService.CreateSerialNumberAsync(dto);
            return CreatedAtAction(nameof(GetBatchSerials), new { batchId = batchId }, serial);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get serial number by code
    /// </summary>
    [HttpGet("serials/{serial}")]
    public async Task<ActionResult<SerialNumberDto>> GetSerialByCode(string serial)
    {
        var serialNumber = await _batchService.GetSerialNumberByCodeAsync(serial);
        if (serialNumber == null)
            return NotFound(new { message = $"Serial {serial} not found" });
        
        return Ok(serialNumber);
    }

    /// <summary>
    /// Approve QC for serial number
    /// </summary>
    [HttpPost("serials/{serialId}/approve-qc")]
    public async Task<ActionResult<SerialNumberDto>> ApproveQc(int serialId, ApproveQcDto dto)
    {
        try
        {
            var serial = await _batchService.ApproveQcAsync(serialId, dto);
            return Ok(serial);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Reject QC for serial number
    /// </summary>
    [HttpPost("serials/{serialId}/reject-qc")]
    public async Task<ActionResult<SerialNumberDto>> RejectQc(int serialId, [FromQuery] string remarks = "")
    {
        try
        {
            var serial = await _batchService.RejectQcAsync(serialId, remarks);
            return Ok(serial);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark serial as shipped
    /// </summary>
    [HttpPost("serials/{serialId}/shipped")]
    public async Task<ActionResult<SerialNumberDto>> MarkAsShipped(int serialId)
    {
        try
        {
            var serial = await _batchService.MarkAsShippedAsync(serialId);
            return Ok(serial);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get audit log for batch
    /// </summary>
    [HttpGet("{batchId}/logs")]
    public async Task<ActionResult<IEnumerable<BatchLogDto>>> GetBatchLogs(int batchId)
    {
        var logs = await _batchService.GetBatchLogsAsync(batchId);
        return Ok(logs);
    }

    /// <summary>
    /// Log batch action (for audit trail)
    /// </summary>
    [HttpPost("logs")]
    public async Task<ActionResult<BatchLogDto>> LogBatchAction(CreateBatchLogDto dto)
    {
        try
        {
            var log = await _batchService.LogBatchActionAsync(dto);
            return CreatedAtAction(nameof(GetBatchLogs), new { batchId = dto.BatchId }, log);
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
}
