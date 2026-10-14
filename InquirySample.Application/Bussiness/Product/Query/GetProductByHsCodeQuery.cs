using InquirySample.Application.Bussiness.Product.Dto;
using InquirySample.Application.Bussiness.Product.Mapper;
using InquirySample.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Application.Bussiness.Product.Query;

public record GetProductByHsCodeQuery : ICacheableQuery<ProductDto>
{
    public required string HsCode { get; init; }

    public string CacheKey => HsCode;
}


public class GetProductByHsCodeQueryHanlder(IReadDbContext readDbContext) : IRequestHandler<GetProductByHsCodeQuery, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(GetProductByHsCodeQuery request, CancellationToken cancellationToken)
    {
        var product = await readDbContext.Products
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync(x => x.HsCode == request.HsCode);

        if (product is null)
            return Error.NotFound(description: "کالا یافت نشد");

        return product;
    }
}
