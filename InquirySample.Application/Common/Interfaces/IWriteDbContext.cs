using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Application.Common.Interfaces
{
    public interface IWriteDbContext
    {
        DbSet<Provider> Providers { get; }
        DbSet<Product> Products { get; }
        DbSet<Inquiry> Inquiries { get; }
        DbSet<ProviderAttempt> ProviderAttempts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
