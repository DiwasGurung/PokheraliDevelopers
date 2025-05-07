using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public int BookId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    public decimal? UnitDiscount { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    // Navigation properties
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }

    [ForeignKey("BookId")]
    public virtual Book Book { get; set; }
}