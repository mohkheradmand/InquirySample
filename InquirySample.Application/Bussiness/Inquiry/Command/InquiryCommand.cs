using InquirySample.Application.Bussiness.Inquiry.Dto;

namespace InquirySample.Application.Bussiness.Inquiry.Command;

public record InquiryCommand : IRequest<ErrorOr<InquiryDto>>
{
    public required string HsCode { get; init; }
}

public class InquiryCommandHandler : IRequestHandler<InquiryCommand, ErrorOr<InquiryDto>>
{
    public Task<ErrorOr<InquiryDto>> Handle(InquiryCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}