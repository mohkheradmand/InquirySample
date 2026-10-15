using InquirySample.BuildingBlocks.Enums;

namespace InquirySample.BuildingBlocks.Dto
{
    public sealed record ProviderResultDto(
    RequestStatus Type,
    ProviderResponseDto Response,
    string? ErrorMessage = null);
}
