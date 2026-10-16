using ErrorOr;
using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Dto;
using InquirySample.Application.Common.Interfaces;
using InquirySample.BuildingBlocks.Enums;
using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InquirySample.Infrastruture.Services.Inquiry
{
    public class InquiryExecutionService(
    IProviderService providerService,
    IProviderHttpClientService providerClient,
    IWriteDbContext writeDbContext,
    IReadDbContext readDbContext,
    IRedisService redis,
    HsCodeLockProvider locks) : IInquiryExecutionService
    {
        public async Task<ErrorOr<InquiryDto>> ExecuteAsync(InquiryDto inquiry, ProductDto product, CancellationToken cancellationToken = default)
        {
            var key = $"inquiry:{product.HsCode}";
            var gate = locks.Get(product.HsCode);

            await gate.WaitAsync(cancellationToken);
            try
            {
                var cached = await redis.GetAsync<CachedInquiryResult>(key, cancellationToken);
                if (cached is not null)
                    return await UseCachedResultAsync(inquiry, cached, cancellationToken);

                return await CallProvidersAsync(inquiry, product, key, cancellationToken);
            }
            finally
            {
                gate.Release();
            }
        }

        private async Task<ErrorOr<InquiryDto>> UseCachedResultAsync(InquiryDto inquiry, CachedInquiryResult cached, CancellationToken cancellationToken)
        {
            // attemptهای نفر اول را بخوان و برای این Inquiry کپی کن
            var sourceAttempts = await readDbContext.ProviderAttempts
                .Where(a => a.InquiryId == cached.SourceInquiryId && a.SharedInquiryId == null)
                .ToListAsync(cancellationToken);

            foreach (var attempt in sourceAttempts)
            {
                await writeDbContext.ProviderAttempts.AddAsync(ProviderAttempt.CreateShared(
                    inquiry.Id,
                    cached.SourceInquiryId,
                    attempt.ProviderId,
                    attempt.RequestStatus,
                    attempt.CreatedAt,
                    attempt.CompletedAt!.Value), cancellationToken);
            }

            await writeDbContext.SaveChangesAsync(cancellationToken);

            if (cached.Status == RequestStatus.Success)
                return inquiry with { Output = cached.Output, ProviderId = cached.ProviderId };

            return Error.Failure(description: "استعلام موفق نبود");
        }

        private async Task<ErrorOr<InquiryDto>> CallProvidersAsync(InquiryDto inquiry, ProductDto product, string key, CancellationToken cancellationToken)
        {
            var providers = providerService.Providers
                .Where(p => p.IsEnabled)
                .OrderBy(p => p.Priority);

            foreach (var provider in providers)
            {
                var attempt = ProviderAttempt.Create(inquiry.Id, provider.Id, DateTime.UtcNow, null);

                var result = await providerClient.ExecuteAsync(provider, product.HsCode, cancellationToken);
                var completedAt = DateTime.UtcNow;

                switch (result.Status)
                {
                    case RequestStatus.Success:
                        attempt.MarkAsSuccess(completedAt);
                        break;
                    case RequestStatus.BussinessFailed:
                        attempt.MarkAsBussinessFailed(completedAt);
                        break;
                    case RequestStatus.Timeout:
                        attempt.MarkAsTimeout(completedAt);
                        break;
                    default:
                        attempt.MarkAsTechnicalError(completedAt);
                        break;
                }

                await writeDbContext.ProviderAttempts.AddAsync(attempt, cancellationToken);
                await writeDbContext.SaveChangesAsync(cancellationToken);

                if (result.Status == RequestStatus.Success)
                {
                    var output = result.Response.Output?.ToString();

                    await redis.WriteAsync(key, new CachedInquiryResult(RequestStatus.Success, output, provider.Id, inquiry.Id),
                        TimeSpan.FromMilliseconds(500), cancellationToken);

                    return inquiry with { Output = output, ProviderId = provider.Id };
                }

                if (result.Status == RequestStatus.BussinessFailed)
                {
                    await redis.WriteAsync(key, new CachedInquiryResult(RequestStatus.BussinessFailed, null, provider.Id, inquiry.Id),
                        TimeSpan.FromMilliseconds(500), cancellationToken);

                    return Error.Failure(description: "استعلام موفق نبود");
                }

                if (result.Status == RequestStatus.Timeout)
                {
                    await redis.WriteAsync(key,
                        new CachedInquiryResult(RequestStatus.Timeout, null, provider.Id, inquiry.Id),
                        TimeSpan.FromMilliseconds(500), cancellationToken);

                    return Error.Failure(description: $"{provider.Name} timed out");
                }

                // Timeout یا TechnicalError → حلقه ادامه پیدا می‌کند (Failover)
            }

            return Error.NotFound(description: "هیچ سرویس‌دهنده‌ای پاسخ نداد");
        }
    }
}
