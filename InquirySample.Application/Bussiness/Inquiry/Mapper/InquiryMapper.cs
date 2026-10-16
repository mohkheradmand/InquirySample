using InquirySample.Application.Bussiness.Inquiry.Dto;

namespace InquirySample.Application.Bussiness.Inquiry.Mapper
{
    public static class InquiryMapper
    {

        public static InquiryDto ToDto(this Domain.Entities.Inquiry entity, string hsCode) =>
            new InquiryDto(entity.Id,
                entity.ProductId,
                hsCode,
                entity.ProviderId,
                entity.CreatedAt,
                entity.CompletedAt,
                entity.Output,
                entity.InquiryStatus
                );
    }
}
