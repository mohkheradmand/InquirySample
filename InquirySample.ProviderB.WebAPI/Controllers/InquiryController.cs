using InquirySample.BuildingBlocks.Dto;
using InquirySample.ProviderB.WebAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.ProviderB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryController(IApplicationDbContext dbContext) : ControllerBase
    {
        [HttpGet("{hsCode}")]
        public async Task<IActionResult> Inquiry(string hsCode, CancellationToken cancellationToken)
        {
            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.HSCode == hsCode,
                    cancellationToken);

            if (product is null)
                return NotFound(new ProviderResponseDto(hsCode, "کالا یافت نشد"));
            

            return Ok(new ProviderResponseDto(hsCode, product.IsLicenseValid.ToString()));
        }
    }
}
