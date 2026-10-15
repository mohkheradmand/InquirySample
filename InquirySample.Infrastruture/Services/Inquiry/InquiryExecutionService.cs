using ErrorOr;
using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Common.Interfaces;
using InquirySample.Domain.Entities;

namespace InquirySample.Infrastruture.Services.Inquiry
{
    public class InquiryExecutionService(IProviderService providerService) : IInquiryExecutionService
    {

        public Task<ErrorOr<InquiryDto>> ExecuteAsync(Product product, CancellationToken cancellationToken = default)
        {
            var providers = providerService.Providers;


            foreach (var provider in providers)
            {
#warning Call Providers
            }
        }
    }
}
