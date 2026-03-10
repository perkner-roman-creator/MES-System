namespace MiniMES.Api.DTOs;

/// <summary>
/// DTO for creating a production batch (výrobní dávka)
/// </summary>
public class CreateBatchDto
{
    public required string BatchNumber { get; set; }
    public int WorkOrderId { get; set; }
    public int? MachineId { get; set; }
    public int? EmployeeId { get; set; }
    public int QuantityPlanned { get; set; }
}

/// <summary>
/// DTO for updating batch information
/// </summary>
public class UpdateBatchDto
{
    public int? QuantityProduced { get; set; }
    public int? QuantityRejected { get; set; }
    public string? Status { get; set; } // Pending, InProduction, QualityCheck, Approved, Rejected, Shipped
}

/// <summary>
/// DTO for returning batch data
/// </summary>
public class BatchDto
{
    public int Id { get; set; }
    public required string BatchNumber { get; set; }
    public int WorkOrderId { get; set; }
    public int? MachineId { get; set; }
    public int? EmployeeId { get; set; }
    public int QuantityPlanned { get; set; }
    public int QuantityProduced { get; set; }
    public int QuantityRejected { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public WorkOrderDto? WorkOrder { get; set; }
    public MachineDto? Machine { get; set; }
    public List<SerialNumberDto> SerialNumbers { get; set; } = new();
    public List<BatchLogDto> BatchLogs { get; set; } = new();
}

/// <summary>
/// DTO for creating serial numbers (sériové čísla) for individual units
/// </summary>
public class CreateSerialNumberDto
{
    public int BatchId { get; set; }
    public required string Serial { get; set; }
    public string? ComponentsUsed { get; set; } // JSON string
}

/// <summary>
/// DTO for QC approval of serial numbers
/// </summary>
public class ApproveQcDto
{
    public required string QcResult { get; set; } // OK or NOK
    public string? QcNotes { get; set; }
    public int? ApprovedByUserId { get; set; }
}

/// <summary>
/// DTO for returning serial number data
/// </summary>
public class SerialNumberDto
{
    public int Id { get; set; }
    public required string Serial { get; set; }
    public int BatchId { get; set; }
    public string Status { get; set; } // InProduction, UnderQualityCheck, QCApproved, QCRejected, Rework, Shipped, Returned
    public string? QcResult { get; set; } // OK or NOK
    public string? QcNotes { get; set; }
    public string? ComponentsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? QcApprovedAt { get; set; }
}

/// <summary>
/// DTO for batch audit log entry
/// </summary>
public class CreateBatchLogDto
{
    public int BatchId { get; set; }
    public required string Action { get; set; } // Created, Started, Paused, Resumed, QualityCheckPassed, QualityCheckFailed, Shipped
    public string? Details { get; set; }
}

/// <summary>
/// DTO for returning batch log data
/// </summary>
public class BatchLogDto
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public required string Action { get; set; }
    public string? Details { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
