// Models/Review.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string Comment { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; }

    public virtual Book Book { get; set; }
}