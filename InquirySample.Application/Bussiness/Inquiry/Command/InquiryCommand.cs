using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Inquiry.Mapper;
using InquirySample.Application.Bussiness.Product.Query;
using InquirySample.Application.Common.Interfaces;

namespace InquirySample.Application.Bussiness.Inquiry.Command;

public record InquiryCommand : IRequest<ErrorOr<InquiryDto>>
{
    public required string HsCode { get; init; }
}

public class InquiryCommandHandler(ISender sender, IInquiryExecutionService inquiryExecutionService, IWriteDbContext writeDbContext) : 
    IRequestHandler<InquiryCommand, ErrorOr<InquiryDto>>
{
    public async Task<ErrorOr<InquiryDto>> Handle(InquiryCommand request, CancellationToken cancellationToken)
    {
        var product = await sender.Send(new GetProductByHsCodeQuery 
        { 
            HsCode = request.HsCode 
        }, cancellationToken);

        if (product.IsError)
            return product.FirstError;

        var inquiry = await sender.Send(new CreateInquiryCommand(product.Value.Id), cancellationToken);

        if (inquiry.IsError)
            return inquiry.FirstError;


        var inquiryExecution = await inquiryExecutionService.ExecuteAsync(inquiry.Value.ToDto(request.HsCode), product.Value, cancellationToken);

        if (inquiryExecution.IsError)
        {
            inquiry.Value.MarkAsFailed(DateTime.UtcNow);

            await UpdateInquiry(inquiry.Value, cancellationToken);

            return inquiry.Value.ToDto(request.HsCode);
        }

        inquiry.Value.MarkAsSuccess(inquiryExecution.Value.Output!, DateTime.UtcNow, inquiryExecution.Value.ProviderId!.Value);

        await UpdateInquiry(inquiry.Value, cancellationToken);

        return inquiry.Value.ToDto(request.HsCode);

    }


    private async Task UpdateInquiry(Domain.Entities.Inquiry inquiry, CancellationToken cancellationToken)
    {
        writeDbContext.Inquiries.Update(inquiry);

        await writeDbContext.SaveChangesAsync(cancellationToken);
    }
}