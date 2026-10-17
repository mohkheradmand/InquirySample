using ErrorOr;
using InquirySample.Application.Bussiness.Inquiry.Command;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InquirySample.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class inquiriesController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Inquiry(InquiryCommand command)
        {
            var inquiry = await sender.Send(command);

            if (inquiry.IsError)
            {
                return CreateErrorResult(inquiry.FirstError);
            }

            return Ok(inquiry.Value);
        }


        private IActionResult CreateErrorResult(Error error)
        {

            return error.Type switch
            {
                ErrorType.NotFound => NotFound(new
                {
                    error.Code,
                    error.Description
                }),

                ErrorType.Validation => BadRequest(new
                {
                    error.Code,
                    error.Description
                }),

                ErrorType.Conflict => Conflict(new
                {
                    error.Code,
                    error.Description
                }),

                _ => StatusCode(500, new
                {
                    error.Code,
                    error.Description
                })
            };
        }
    }
    
}
