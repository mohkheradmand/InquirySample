using InquirySample.BuildingBlocks.Dto;
using InquirySample.ProviderB.WebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InquirySample.ProviderB.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryController(IApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Inquiry([FromBody] ProviderRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);

                var product = await dbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.HSCode == request.HsCode,
                        cancellationToken);

                if (product is null)
                    return NotFound(new ProviderResponseDto(request.HsCode, "کالا یافت نشد"));


                return Ok(new ProviderResponseDto(request.HsCode, product.IsLicenseValid.ToString()));
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
