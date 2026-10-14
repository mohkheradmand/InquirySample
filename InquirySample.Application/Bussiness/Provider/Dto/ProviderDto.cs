namespace InquirySample.Application.Bussiness.Provider.Dto;

public record ProviderDto(
    int Id, 
    string Name, 
    int Priority, 
    string BaseUrl, 
    int TimeoutMs, 
    bool IsEnabled
    );
