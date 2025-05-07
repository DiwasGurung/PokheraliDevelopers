using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PokheraliDevelopers.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }

    [Required]
    public string OrderNumber { get; set; } // For reference in emails/UI

    [Required]
    public decimal SubTotal { get; set; }

    public decimal? DiscountAmount { get; set; }
    public string DiscountCode { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public string OrderStatus { get; set; } // "Pending", "Processing", "Shipped", "Delivered", "Cancelled"

    public string ShippingAddress { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
    public string ShippingZipCode { get; set; }

    [Required]
    public string PaymentStatus { get; set; } // "Pending", "Completed", "Failed", "Refunded"

    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }

    // Claim code for order fulfillment by staff
    public string ClaimCode { get; set; }
    public bool IsClaimCodeUsed { get; set; } = false;
    public DateTime? ClaimCodeUsedAt { get; set; }
    public string ClaimCodeUsedByStaffId { get; set; }

    // Track if customer received a volume discount (5+ books)
    public bool ReceivedVolumeDiscount { get; set; } = false;

    // Track if customer received a loyalty discount (10+ successful orders)
    public bool ReceivedLoyaltyDiscount { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}