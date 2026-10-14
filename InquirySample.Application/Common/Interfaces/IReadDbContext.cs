using InquirySample.Domain.Entities;

namespace InquirySample.Application.Common.Interfaces;

public interface IReadDbContext
{
    IQueryable<Provider> Providers { get; }
    IQueryable<Product> Products { get; }
    IQueryable<Inquiry> Inquiries { get; }
    IQueryable<ProviderAttempt> ProviderAttempts { get; }
}
