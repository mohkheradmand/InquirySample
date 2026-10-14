using InquirySample.Application.Bussiness.Provider.Dto;

namespace InquirySample.Application.Common.Interfaces;

public interface IProviderService
{
    IReadOnlyList<ProviderDto> Providers { get; }

    Task Load();
}
