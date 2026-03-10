using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Api.DTOs;
using MiniMES.Api.Models;
using MiniMES.Api.Services;

namespace MiniMES.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductionLogsController : ControllerBase
{
    private readonly IProductionLogService _productionLogService;

    public ProductionLogsController(IProductionLogService productionLogService)
    {
        _productionLogService = productionLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductionLog>>> GetAll()
    {
        var logs = await _productionLogService.GetAllLogsAsync();
        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductionLog>> GetById(int id)
    {
        var log = await _productionLogService.GetLogByIdAsync(id);
        if (log == null)
        {
            return NotFound();
        }
        return Ok(log);
    }

    [HttpGet("workorder/{workOrderId}")]
    public async Task<ActionResult<IEnumerable<ProductionLog>>> GetByWorkOrderId(int workOrderId)
    {
        var logs = await _productionLogService.GetLogsByWorkOrderIdAsync(workOrderId);
        return Ok(logs);
    }

    [HttpPost]
    public async Task<ActionResult<ProductionLog>> Create([FromBody] CreateProductionLogDto dto)
    {
        var log = await _productionLogService.CreateLogAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = log.Id }, log);
    }
}
