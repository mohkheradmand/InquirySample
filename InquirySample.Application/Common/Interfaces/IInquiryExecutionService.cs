using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Domain.Entities;

namespace InquirySample.Application.Common.Interfaces
{
    public interface IInquiryExecutionService
    {
        Task<ErrorOr<InquiryDto>> ExecuteAsync(
        Product product,
        CancellationToken cancellationToken = default);
    }
}
