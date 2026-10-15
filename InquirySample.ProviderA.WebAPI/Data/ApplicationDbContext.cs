using InquirySample.ProviderA.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.ProviderA.WebAPI.Data;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set;  }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {

            entity.HasKey(x => x.Id);

            entity.Property(x => x.HSCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => x.HSCode)
                .IsUnique();

            entity.Property(x => x.IsLicenseValid)
                .IsRequired();
        });
    }
}