namespace MiniMES.Api.Models;

/// <summary>
/// Výrobní dávka (batch) pro sledování výroby
/// Umožňuje sledovat komponenty a třídit výrobky podle seriálního čísla
/// </summary>
public class ProductBatch
{
    public int Id { get; set; }
    public required string BatchNumber { get; set; }  // Identifikátor dávky (unique)
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    
    public int? MachineId { get; set; }
    public Machine? Machine { get; set; }
    
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    public int QuantityPlanned { get; set; }
    public int QuantityProduced { get; set; } = 0;
    public int QuantityRejected { get; set; } = 0;
    
    public BatchStatus Status { get; set; } = BatchStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();
    public ICollection<BatchLog> BatchLogs { get; set; } = new List<BatchLog>();
}

/// <summary>
/// Sériové číslo jednotlivého kusu v dávce
/// Umožňuje sledovat každý kus zvlášť
/// </summary>
public class SerialNumber
{
    public int Id { get; set; }
    public required string Serial { get; set; }  // Sériové číslo (unique)
    public int BatchId { get; set; }
    public ProductBatch? Batch { get; set; }
    
    public SerialStatus Status { get; set; } = SerialStatus.InProduction;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? QcResult { get; set; }  // OK/NOK
    public string? QcNotes { get; set; }
    
    // Traceability
    public string? ComponentsUsed { get; set; }  // JSON seznam komponent
    public int? QcApprovedBy { get; set; }
    public DateTime? QcApprovedAt { get; set; }
}

public enum BatchStatus
{
    Pending = 0,
    InProduction = 1,
    QualityCheck = 2,
    Approved = 3,
    Rejected = 4,
    Shipped = 5
}

public enum SerialStatus
{
    InProduction = 0,
    UnderQualityCheck = 1,
    QCApproved = 2,
    QCRejected = 3,
    Rework = 4,
    Shipped = 5,
    Returned = 6
}

/// <summary>
/// Audit log pro dávku (sleduje veškeré změny)
/// </summary>
public class BatchLog
{
    public int Id { get; set; }
    public int BatchId { get; set; }
    public ProductBatch? Batch { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
