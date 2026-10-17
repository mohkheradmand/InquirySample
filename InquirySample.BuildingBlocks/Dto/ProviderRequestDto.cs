using System.ComponentModel.DataAnnotations;

namespace InquirySample.BuildingBlocks.Dto;

public sealed record ProviderRequestDto
{
    [Required(ErrorMessage = "HsCode نباید خالی باشد")]
    public required string HsCode { get; init; }
}
