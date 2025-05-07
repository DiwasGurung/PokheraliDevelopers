// DTOs/CartDto.cs
using System.Collections.Generic;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool QualifiesForBulkDiscount { get; set; }
    public bool HasStackableDiscount { get; set; }
}