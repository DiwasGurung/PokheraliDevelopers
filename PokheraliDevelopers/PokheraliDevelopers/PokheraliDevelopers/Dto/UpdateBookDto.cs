﻿// DTOs/UpdateBookDto.cs
using System.ComponentModel.DataAnnotations;

public class UpdateBookDto
{
    [MaxLength(255)]
    public string Title { get; set; }

    [MaxLength(50)]
    public string ISBN { get; set; }

    public string Description { get; set; }

    [MaxLength(255)]
    public string Author { get; set; }

    public string Publisher { get; set; }

    public DateTime? PublicationDate { get; set; }

    [Range(0.01, 10000)]
    public decimal? Price { get; set; }

    [Range(0, 10000)]
    public int? StockQuantity { get; set; }

    public string Language { get; set; }

    public string Format { get; set; }

    public string Genre { get; set; }

    public string ImageUrl { get; set; }

    [Range(0, 100)]
    public decimal? DiscountPercentage { get; set; }

    public bool? IsOnSale { get; set; }

    public DateTime? DiscountStartDate { get; set; }

    public DateTime? DiscountEndDate { get; set; }
}