using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PokheraliDevelopers.Models;

public class Bookmark
{
    [Key]
    public int Id { get; set; }

    public int BookId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("BookId")]
    public virtual Book Book { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
}
