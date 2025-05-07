using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Book
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Author { get; set; }
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public int Stock { get; set; }
    [Required]
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public string Publisher { get; set; }
    public DateTime? PublishDate { get; set; }
    public string Language { get; set; } = "English";
    public string Format { get; set; } = "Paperback";
    public int? Pages { get; set; }
    public string Dimensions { get; set; }
    public string Weight { get; set; }
    public string ImageUrl { get; set; }
    // Timestamp for when the record was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Timestamp for when the record was last updated
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Additional properties for sales and discounts
    public bool IsBestseller { get; set; } = false;
    public bool IsNewRelease { get; set; } = false;
    public bool IsOnSale { get; set; } = false;
    public decimal? DiscountPercentage { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
    public decimal? OriginalPrice { get; set; }

    // Navigation properties
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }
    public virtual ICollection<Bookmark> Bookmarks { get; set; }
}