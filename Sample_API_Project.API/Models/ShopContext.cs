using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Sample_API_Project.API.Models;

public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(c => c.Category)
            .HasForeignKey(c => c.CategoryId);

        modelBuilder.Seed();
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
