using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) 
    {
        Database.EnsureCreated();
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Book> Books { get; set; }

    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
                    .HasMany(c => c.Books)
                    .WithOne(b => b.Category)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books)
            .UsingEntity(j => j.ToTable("BookTags"));
                    

        base.OnModelCreating(modelBuilder);
    }
}