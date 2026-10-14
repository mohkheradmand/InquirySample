using InquirySample.Application.Bussiness.Product.Dto;

namespace InquirySample.Application.Bussiness.Product.Mapper;

public static class ProductMapper
{

    public static ProductDto ToDto(this Domain.Entities.Product entity) =>
        new ProductDto(entity.Id, entity.HsCode);
}
