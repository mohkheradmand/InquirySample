using InquirySample.Application.Bussiness.Provider.Dto;
using InquirySample.Application.Common.Interfaces;
using InquirySample.BuildingBlocks.Dto;
using InquirySample.BuildingBlocks.Enums;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace InquirySample.Infrastruture.Services.Provider
{
    public sealed class ProviderHttpClientService(IHttpClientFactory httpClientFactory) : IProviderHttpClientService
    {
        public async Task<ProviderResultDto> ExecuteAsync(ProviderDto provider, string hsCode, CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient();

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            timeoutCts.CancelAfter(TimeSpan.FromMilliseconds(provider.TimeoutMs));

            try
            {
                var url = $"{provider.BaseUrl.TrimEnd('/')}/api/inquiry";

                using var response = await client.PostAsJsonAsync(url, new ProviderRequestDto { HsCode = hsCode }, timeoutCts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    var status = MapFailureStatus(response.StatusCode);

                    return Fail(status, hsCode,
                        status == RequestStatus.Timeout ?
                            $"{provider.Name} timed out : {response.Content.ToString()}" :
                            $"{provider.Name} returned HTTP {(int)response.StatusCode} : {response.Content.ToString()}");
                }

                var result = await response.Content.ReadFromJsonAsync<ProviderResponseDto>(timeoutCts.Token);

                if (result is null || string.IsNullOrWhiteSpace(result.HsCode) || result.Output is null)
                    return Fail(RequestStatus.TechnicalError, hsCode, $"{provider.Name} پاسخ نامعتبر داد");
                

                return new ProviderResultDto(RequestStatus.Success,Response: result);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return Fail(RequestStatus.Timeout, hsCode, $"{provider.Name} timed out");
            }
            catch (HttpRequestException)
            {
                return Fail(RequestStatus.TechnicalError, hsCode, $"{provider.Name} در دسترس نیست");
            }
            catch (JsonException)
            {
                return Fail(RequestStatus.TechnicalError, hsCode, $"{provider.Name} پاسخ نامعتبر داد");
            }
        }

        private RequestStatus MapFailureStatus(HttpStatusCode code) => code switch
        {
            HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout => RequestStatus.Timeout,
            HttpStatusCode.BadRequest => RequestStatus.BussinessFailed,
            _ => RequestStatus.TechnicalError
        };

        private ProviderResultDto Fail(RequestStatus status, string hsCode, string message) =>
            new(status, new ProviderResponseDto(hsCode, null), message);
    }
}
