using InquirySample.Application.Common.Interfaces;

namespace InquirySample.Application.Bussiness.Inquiry.Command
{
    public record CreateInquiryCommand(int ProductId) : IRequest<ErrorOr<int>>;

    public class CreateInquiryCommandHandler(IWriteDbContext writeDbContext) : IRequestHandler<CreateInquiryCommand, ErrorOr<int>>
    {
        public async Task<ErrorOr<int>> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
        {
            var inquiry = Domain.Entities.Inquiry.Create(request.ProductId, DateTime.UtcNow);

            await writeDbContext.Inquiries.AddAsync(inquiry, cancellationToken);

            await writeDbContext.SaveChangesAsync(cancellationToken);

            return inquiry.Id;
        }
    }
}
