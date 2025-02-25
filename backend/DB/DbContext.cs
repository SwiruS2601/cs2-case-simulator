using Cs2CaseOpener.Models;
using Microsoft.EntityFrameworkCore;

namespace Cs2CaseOpener.DB;

public class ApplicationDbContext : DbContext
{
    public DbSet<Skin> Skins { get; set; }
    public DbSet<Crate> Crates { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Rarity> Rarities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Crate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(120).HasColumnName("name");
            entity.Property(e => e.Description).HasMaxLength(1000).HasColumnName("description");
            entity.Property(e => e.Type).HasMaxLength(50).HasColumnName("type");
            entity.Property(e => e.Market_Hash_Name).HasMaxLength(100).HasColumnName("market_hash_name");
            entity.Property(e => e.Image).HasMaxLength(400).HasColumnName("image");
            entity.Property(e => e.Model_Player).HasMaxLength(150).HasColumnName("model_player");
            entity.HasIndex(e => e.Name);
        });
        
        modelBuilder.Entity<Skin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(120).HasColumnName("name");
            entity.Property(e => e.RarityId).HasMaxLength(50).HasColumnName("rarity_id");
            entity.Property(e => e.PaintIndex).HasMaxLength(10).HasColumnName("paint_index");
            entity.Property(e => e.Image).HasMaxLength(400).HasColumnName("image");
            entity.HasIndex(e => e.Name);
            
            entity.HasOne(s => s.Rarity)
                  .WithMany(r => r.Skins)
                  .HasForeignKey(s => s.RarityId);
        });
        
        modelBuilder.Entity<Rarity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(120).HasColumnName("name");
            entity.Property(e => e.Color).HasMaxLength(7).HasColumnName("color");
            entity.HasIndex(e => e.Name);
        });
        
        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SkinId).HasMaxLength(50).HasColumnName("skinId");
            entity.Property(e => e.Wear_Category).HasMaxLength(50).HasColumnName("wear_category");
            entity.HasIndex(e => new { e.SkinId, e.Wear_Category }).IsUnique();
            
            entity.HasOne(e => e.Skin)
                  .WithMany(s => s.Prices)
                  .HasForeignKey(e => e.SkinId);
        });
        
        modelBuilder.Entity<Crate>()
            .HasMany(c => c.Skins)
            .WithMany(s => s.Crates)
            .UsingEntity(j => j.ToTable("CrateSkins"));
    }
}

