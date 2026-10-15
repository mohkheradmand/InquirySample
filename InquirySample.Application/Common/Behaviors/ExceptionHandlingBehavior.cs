using InquirySample.Application.Common.Interfaces;
using InquirySample.Domain.Entities;
using InquirySample.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Application.Common.Behaviors
{
    public class ExceptionHandlingBehavior<TRequest, TResponse>(IWriteDbContext writeDbContext) :
    IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : IRequest<ErrorOr<TResponse>>
    {
        public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
        {
            try
            {
                var result = await next(cancellationToken);

                if (result.IsError)
                    await SetErrorLog(result.Errors!.First().Description, cancellationToken);

                return result;
            }
            catch (DomainException ex)
            {
                await SetErrorLog(ex.Message, cancellationToken);

                return Error.Validation(
                    code: "Domain.Validation",
                    description: ex.Message);
            }
            catch (DbUpdateException ex)
            {
                await SetErrorLog(ex.Message, cancellationToken);

                return Error.Failure(
                    code: "Database.Error",
                    description: "در ذخیره ‌سازی دیتابیس مشکلی پیش آمده است");
            }
            catch (Exception ex)
            {
                await SetErrorLog(ex.Message, cancellationToken);

                return Error.Unexpected(
                    code: "Server.Unexpected",
                    description: "خطای غیرمنتظره‌ای رخ داده است");
            }
        }

        private async Task SetErrorLog(string msg, CancellationToken cancellationToken)
        {
            var errorLog = ErrorLog.Create(typeof(TRequest).Name, msg);

            await writeDbContext.ErrorLogs.AddAsync(errorLog, cancellationToken);

            await writeDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
