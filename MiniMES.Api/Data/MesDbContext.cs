using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Models;

namespace MiniMES.Api.Data;

public class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<ProductionLog> ProductionLogs { get; set; }
    public DbSet<Operation> Operations { get; set; }
    public DbSet<WorkOrderOperation> WorkOrderOperations { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }
    public DbSet<ProductBatch> ProductBatches { get; set; }
    public DbSet<SerialNumber> SerialNumbers { get; set; }
    public DbSet<BatchLog> BatchLogs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
        });
        
        // WorkOrder configuration
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            
            entity.HasOne(e => e.AssignedMachine)
                .WithMany(m => m.WorkOrders)
                .HasForeignKey(e => e.AssignedMachineId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.AssignedEmployee)
                .WithMany(emp => emp.WorkOrders)
                .HasForeignKey(e => e.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Machine configuration
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MachineCode).IsUnique();
            entity.Property(e => e.MachineCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
        });
        
        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmployeeCode).IsUnique();
            entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        });
        
        // ProductionLog configuration
        modelBuilder.Entity<ProductionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.WorkOrder)
                .WithMany(wo => wo.ProductionLogs)
                .HasForeignKey(e => e.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Machine)
                .WithMany(m => m.ProductionLogs)
                .HasForeignKey(e => e.MachineId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Employee)
                .WithMany(emp => emp.ProductionLogs)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Operation configuration
        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OperationCode).IsUnique();
            entity.Property(e => e.OperationCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasOne(e => e.RequiredMachine)
                .WithMany()
                .HasForeignKey(e => e.RequiredMachineId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // WorkOrderOperation configuration
        modelBuilder.Entity<WorkOrderOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Composite unique index for WorkOrder + Operation
            entity.HasIndex(e => new { e.WorkOrderId, e.OperationId }).IsUnique();
            
            entity.HasOne(e => e.WorkOrder)
                .WithMany(wo => wo.WorkOrderOperations)
                .HasForeignKey(e => e.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Operation)
                .WithMany()
                .HasForeignKey(e => e.OperationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // OperationLog configuration
        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.WorkOrderOperation)
                .WithMany(wo => wo.OperationLogs)
                .HasForeignKey(e => e.WorkOrderOperationId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Machine)
                .WithMany()
                .HasForeignKey(e => e.MachineId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // ProductBatch configuration
        modelBuilder.Entity<ProductBatch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BatchNumber).IsUnique();
            entity.Property(e => e.BatchNumber).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.WorkOrder)
                .WithMany(wo => wo.ProductBatches)
                .HasForeignKey(e => e.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Machine)
                .WithMany()
                .HasForeignKey(e => e.MachineId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // SerialNumber configuration
        modelBuilder.Entity<SerialNumber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Serial).IsUnique();
            entity.Property(e => e.Serial).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.Batch)
                .WithMany(b => b.SerialNumbers)
                .HasForeignKey(e => e.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // BatchLog configuration
        modelBuilder.Entity<BatchLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Batch)
                .WithMany(b => b.BatchLogs)
                .HasForeignKey(e => e.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
