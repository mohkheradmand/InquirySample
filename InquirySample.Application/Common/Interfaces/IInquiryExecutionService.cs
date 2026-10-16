using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Dto;

namespace InquirySample.Application.Common.Interfaces
{
    public interface IInquiryExecutionService
    {
        Task<ErrorOr<InquiryDto>> ExecuteAsync(InquiryDto inquiry, ProductDto product, CancellationToken cancellationToken = default);
    }
}
