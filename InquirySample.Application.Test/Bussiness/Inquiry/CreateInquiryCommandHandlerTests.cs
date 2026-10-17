using InquirySample.Application.Bussiness.Inquiry.Command;
using InquirySample.Application.Test.Helper;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Application.Test.Bussiness.Inquiry
{
    public class CreateInquiryCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesAndPersistsInquiry()
        {
            await using var db = DbContextFactory.Create();
            var handler = new CreateInquiryCommandHandler(db);

            var result = await handler.Handle(new CreateInquiryCommand(5), CancellationToken.None);

            result.IsError.Should().BeFalse();
            result.Value.ProductId.Should().Be(5);
            (await db.Inquiries.CountAsync()).Should().Be(1);
        }
    }
}
