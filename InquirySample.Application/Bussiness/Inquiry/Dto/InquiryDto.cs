using InquirySample.Domain.Enums;

namespace InquirySample.Application.Bussiness.Inquiry.Dto
{
    public record InquiryDto(int Id,
        int ProductId,
        string HsCode,
        int? ProviderId,
        DateTime CreatedAt,
        DateTime? CompletedAt,
        string? Output,
        InquiryStatus InquiryStatus
        )
    {
        public string Status => InquiryStatus.ToString();
    }
}
