using InquirySample.Domain.Entities;

namespace InquirySample.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    IQueryable<Provider> Providers { get; }
}
