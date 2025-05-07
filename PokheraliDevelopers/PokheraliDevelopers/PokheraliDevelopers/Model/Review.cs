using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PokheraliDevelopers.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    public int BookId { get; set; }
    public string UserId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(1000)]
    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("BookId")]
    public virtual Book Book { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
}