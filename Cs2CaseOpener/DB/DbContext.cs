using Cs2CaseOpener.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.DB;

public class ApplicationDbContext : DbContext
{
    public DbSet<Skin> Skins { get; set; }
    public DbSet<Case> Cases { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Map entities to tables.
        modelBuilder.Entity<Case>().ToTable("Cases");
        modelBuilder.Entity<Skin>().ToTable("Skins");
        
        // Configure many-to-many relationship
        modelBuilder.Entity<Case>()
            .HasMany(c => c.Skins)
            .WithMany(s => s.Cases)
            .UsingEntity(j => j.ToTable("CasesSkins"));
        
        // Create indexes to optimize read-heavy workloads.
        modelBuilder.Entity<Case>().HasIndex(c => c.Name);
        modelBuilder.Entity<Skin>().HasIndex(s => s.Name);
    }
}

