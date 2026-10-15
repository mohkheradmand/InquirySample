using InquirySample.Application.Bussiness.Provider.Dto;
using InquirySample.Application.Common.Interfaces;
using InquirySample.BuildingBlocks.Dto;
using InquirySample.BuildingBlocks.Enums;
using System.Net;
using System.Net.Http.Json;

namespace InquirySample.Infrastruture.Services.Provider
{
    public sealed class ProviderHttpClient(
    IHttpClientFactory httpClientFactory) : IProviderHttpClientService
    {
        public async Task<ProviderResultDto> ExecuteAsync(
            ProviderDto provider,
            string hsCode,
            CancellationToken cancellationToken = default)
        {
            using var client = httpClientFactory.CreateClient();

            using var timeoutCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            timeoutCts.CancelAfter(
                TimeSpan.FromMilliseconds(provider.TimeoutMs));

            try
            {
                var url = $"{provider.BaseUrl.TrimEnd('/')}/api/inquiry";

                using var response = await client.PostAsJsonAsync(
                    url,
                    new ProviderRequestDto(hsCode),
                    timeoutCts.Token);

                if (response.StatusCode == HttpStatusCode.RequestTimeout ||
                    (int)response.StatusCode == 504)
                {
                    return new ProviderResultDto(
                        RequestStatus.Timeout,
                        new ProviderResponseDto(hsCode, null),
                        $"{provider.Name} timed out.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var type = (int)response.StatusCode >= 500
                        ? RequestStatus.TechnicalError
                        : RequestStatus.BussinessFailed;

                    return new ProviderResultDto(
                        type,
                        new ProviderResponseDto(hsCode, null),
                        $"{provider.Name} returned HTTP {(int)response.StatusCode}.");
                }

                var result =
                    await response.Content.ReadFromJsonAsync<ProviderResponseDto>(
                        cancellationToken: timeoutCts.Token);

                if (result is null ||
                    string.IsNullOrWhiteSpace(result.HsCode) ||
                    result.Output is null)
                {
                    return new ProviderResultDto(
                        RequestStatus.TechnicalError,
                        new ProviderResponseDto(hsCode, null),
                        $"{provider.Name} پاسخ نامعتبر داد");
                }

                return new ProviderResultDto(
                    RequestStatus.Success,
                    Response: result);
            }
            catch (OperationCanceledException)
            {
                return new ProviderResultDto(
                    RequestStatus.Timeout,
                    new ProviderResponseDto(hsCode, null),
                    $"{provider.Name} timed out.");
            }
            catch (HttpRequestException ex)
            {
                return new ProviderResultDto(
                    RequestStatus.TechnicalError,
                    new ProviderResponseDto(hsCode, null),
                    ex.Message);
            }
            catch (System.Text.Json.JsonException ex)
            {
                return new ProviderResultDto(
                    RequestStatus.TechnicalError,
                    new ProviderResponseDto(hsCode, null),
                    ex.Message);
            }
        }
    }
}
