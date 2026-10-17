using InquirySample.Application.Bussiness.Product.Query;
using InquirySample.Application.Test.Helper;

namespace InquirySample.Application.Test.Bussiness.Product
{
    public class GetProductByHsCodeQueryHandlerTests
    {
        [Fact]
        public async Task Handle_WhenProductExists_ReturnsProductDto()
        {
            // Arrange
            await using var db = DbContextFactory.Create();
            db.Products.Add(new Domain.Entities.Product { HsCode = "123456" , Id = 1});
            await db.SaveChangesAsync();

            var handler = new GetProductByHsCodeQueryHanlder(db);

            // Act
            var result = await handler.Handle(
                new GetProductByHsCodeQuery { HsCode = "123456" }, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.HsCode.Should().Be("123456");
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_ReturnsNotFound()
        {
            await using var db = DbContextFactory.Create();
            var handler = new GetProductByHsCodeQueryHanlder(db);

            var result = await handler.Handle(
                new GetProductByHsCodeQuery { HsCode = "000000" }, CancellationToken.None);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
        }
    }
}
