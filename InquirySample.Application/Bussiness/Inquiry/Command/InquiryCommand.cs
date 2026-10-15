using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Query;

namespace InquirySample.Application.Bussiness.Inquiry.Command;

public record InquiryCommand : IRequest<ErrorOr<InquiryDto>>
{
    public required string HsCode { get; init; }
}

public class InquiryCommandHandler(ISender sender) : IRequestHandler<InquiryCommand, ErrorOr<InquiryDto>>
{
    public async Task<ErrorOr<InquiryDto>> Handle(InquiryCommand request, CancellationToken cancellationToken)
    {
        var startedTime = DateTime.UtcNow;

        var product = await sender.Send(new GetProductByHsCodeQuery { HsCode = request.HsCode });

        if (product.IsError)
            return product.FirstError;

#warning TODO: Call InquiryExecutionService
    }
}