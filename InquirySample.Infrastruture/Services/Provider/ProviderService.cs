using InquirySample.Application.Bussiness.Provider.Dto;
using InquirySample.Application.Bussiness.Provider.Mapper;
using InquirySample.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InquirySample.Infrastruture.Services.Providerl;

public class ProviderService : IProviderService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private List<ProviderDto> _providers = [];

    public IReadOnlyList<ProviderDto> Providers => _providers;

    public ProviderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Load()
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<IReadDbContext>();

        _providers = await dbContext.Providers.Select(x => x.ToDto()).ToListAsync();
    }
}
