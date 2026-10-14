using InquirySample.Application.Bussiness.Provider.Dto;

namespace InquirySample.Application.Bussiness.Provider.Mapper
{
    public static class ProviderMapper
    {

        public static ProviderDto ToDto(this Domain.Entities.Provider entity) =>
            new ProviderDto(entity.Id,
                            entity.Name,
                            entity.Priority,
                            entity.BaseUrl,
                            entity.TimeoutMs,
                            entity.IsEnabled);
    }
}
