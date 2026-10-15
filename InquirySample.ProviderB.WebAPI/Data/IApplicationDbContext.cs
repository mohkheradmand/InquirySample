using InquirySample.ProviderB.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.ProviderB.WebAPI.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
