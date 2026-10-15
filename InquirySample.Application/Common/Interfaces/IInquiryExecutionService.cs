using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Dto;

namespace InquirySample.Application.Common.Interfaces
{
    public interface IInquiryExecutionService
    {
        Task<ErrorOr<InquiryDto>> ExecuteAsync(int inquiryId, ProductDto product, CancellationToken cancellationToken = default);
    }
}
