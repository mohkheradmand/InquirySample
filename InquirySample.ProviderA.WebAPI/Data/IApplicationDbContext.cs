using InquirySample.ProviderA.WebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.ProviderA.WebAPI.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
