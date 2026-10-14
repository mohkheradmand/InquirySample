using InquirySample.Application.Common.Interfaces;
using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace InquirySample.Infrastruture.Persistance;

public class ApplicationDbContext : DbContext, IReadDbContext, IWriteDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<ProviderAttempt> ProviderAttempts { get; set; }

    IQueryable<Provider> IReadDbContext.Providers => Providers.AsNoTracking();

    IQueryable<Product> IReadDbContext.Products => Products.AsNoTracking();

    IQueryable<Inquiry> IReadDbContext.Inquiries => Inquiries.AsNoTracking();

    IQueryable<ProviderAttempt> IReadDbContext.ProviderAttempts => ProviderAttempts.AsNoTracking();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

}
