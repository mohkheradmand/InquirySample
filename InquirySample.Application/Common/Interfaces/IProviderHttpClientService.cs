using InquirySample.Application.Bussiness.Provider.Dto;
using InquirySample.BuildingBlocks.Dto;

namespace InquirySample.Application.Common.Interfaces;

public interface IProviderHttpClientService
{
    Task<ProviderResultDto> ExecuteAsync(ProviderDto provider, string hsCode, CancellationToken cancellationToken = default);
}
