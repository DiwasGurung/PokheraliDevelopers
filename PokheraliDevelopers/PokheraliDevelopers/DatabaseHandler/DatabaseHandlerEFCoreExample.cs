using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseHandlerEfCoreExample : IdentityDbContext<IdentityUser>
{
    public DatabaseHandlerEfCoreExample(DbContextOptions<DatabaseHandlerEfCoreExample> options)
        : base(options)
    {
    }

    // Add Books DbSet
    public DbSet<Book> Books { get; set; }

    // Optional: Configure any additional model mappings or constraints
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Example: Configure Book entity
        builder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.Genre).HasMaxLength(100);
        });
    }
}