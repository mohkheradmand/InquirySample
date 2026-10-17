using InquirySample.BuildingBlocks.Dto;
using InquirySample.ProviderA.WebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InquirySample.ProviderA.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryController(IApplicationDbContext dbContext) : ControllerBase
    {
        private readonly Regex _regex = new("^[0-9]{5}[0123578]$");

        [HttpPost]
        public async Task<IActionResult> Inquiry([FromBody] ProviderRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ValidationProblem(ModelState);


                if (!_regex.IsMatch(request.HsCode))
                    return BadRequest("رقم آخر نباید 4 6 9 باشد");

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
