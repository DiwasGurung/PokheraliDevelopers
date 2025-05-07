// Models/Order.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string ClaimCode { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public bool BulkDiscount { get; set; } = false;

    public bool StackableDiscount { get; set; } = false;

    public string StaffId { get; set; }

    public DateTime? ProcessedDate { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; }

    [ForeignKey("StaffId")]
    public virtual IdentityUser Staff { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}