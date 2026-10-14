using InquirySample.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;

namespace InquirySample.Infrastruture.Services.Provider
{
    internal class ProviderServiceInitializer(IProviderService providerService) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await providerService.Load();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
