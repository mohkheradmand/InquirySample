using ErrorOr;
using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Dto;
using InquirySample.Application.Common.Interfaces;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace InquirySample.Infrastruture.Services.Inquiry
{
    public class InquiryExecutionService(IProviderService providerService) : IInquiryExecutionService
    {
        public Task<ErrorOr<InquiryDto>> ExecuteAsync(int inquiryId, ProductDto product, CancellationToken cancellationToken = default)
        {
            var providers = providerService.Providers;


            foreach (var provider in providers)
            {
#warning Call Providers
            }
        }
    }
}
