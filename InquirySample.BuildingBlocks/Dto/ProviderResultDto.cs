using InquirySample.BuildingBlocks.Enums;

namespace InquirySample.BuildingBlocks.Dto
{
    public sealed record ProviderResultDto(
    RequestStatus Status,
    ProviderResponseDto Response,
    string? ErrorMessage = null);
}
