using Microsoft.EntityFrameworkCore;
using EmlakSitesi.Models.Entities;

namespace EmlakSitesi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<About> Abouts => Set<About>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<District> Districts => Set<District>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // 1. İlan - Fotoğraf İlişkisi (İlan silinince fotoğraflar silinir)
        mb.Entity<Property>()
          .HasMany(p => p.Images)
          .WithOne(i => i.Property)
          .HasForeignKey(i => i.PropertyId)
          .OnDelete(DeleteBehavior.Cascade);

        // 3. İl/ilçe silinince ilanlar silinmesin
        mb.Entity<Property>()
          .HasOne(p => p.City).WithMany()
          .HasForeignKey(p => p.CityId)
          .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Property>()
          .HasOne(p => p.District).WithMany()
          .HasForeignKey(p => p.DistrictId)
          .OnDelete(DeleteBehavior.Restrict);

        // 4. Benzersiz indeksler
        mb.Entity<Property>().HasIndex(p => p.Slug).IsUnique();
        mb.Entity<Setting>().HasIndex(s => s.Key).IsUnique();
        mb.Entity<User>().HasIndex(u => u.Username).IsUnique();

        // 5. Hassasiyet ayarları
        mb.Entity<Property>().Property(p => p.Price).HasPrecision(18, 2);
        mb.Entity<Property>().Property(p => p.Dues).HasPrecision(18, 2);
    }
}