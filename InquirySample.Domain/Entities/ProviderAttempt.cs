using InquirySample.Domain.Enums;
using InquirySample.Domain.Exceptions;

namespace InquirySample.Domain.Entities;

public class ProviderAttempt
{
    public int Id { get; private set; }
    public int InquiryId { get; private set; }
    public Inquiry Inquiry { get; private set; }
    public int? SharedInquiryId { get; private set; }
    public Inquiry? SharedInquiry { get; private set; }
    public int ProviderId { get; private set; }
    public Provider Provider { get; private set; }
    public RequestStatus RequestStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ProviderAttempt()
    {
    }

    private ProviderAttempt(int inquiryId, int providerId, DateTime createdAt, int? sharedInquiryId)
    {
        SetInquiryId(inquiryId);
        SetProviderId(providerId);

        CreatedAt = createdAt;
        SharedInquiryId = sharedInquiryId;
        RequestStatus = RequestStatus.Pending;
    }

    public static ProviderAttempt Create(int inquiryId, int providerId, DateTime createdAt, int? sharedInquiryId)
    {
        return new ProviderAttempt(inquiryId, providerId, createdAt, sharedInquiryId);
    }


    private void SetInquiryId(int inquiryId)
    {
        if (inquiryId < 0)
            throw new DomainException("شناسه استعلام نباید 0 یا کوچکتر باشد");

        InquiryId = inquiryId;
    }

    private void SetProviderId(int providerId)
    {
        if (providerId < 0)
            throw new DomainException("شناسه سرویس دهنده نباید 0 یا کوچکتر باشد");

       ProviderId = providerId;
    }



    public void SetCompeleteAt(DateTime completedAt) => CompletedAt = completedAt;
    public void SetRequestStatusToSuccess() => RequestStatus = RequestStatus.Success;
    public void SetRequestStatusToTechnicalError() => RequestStatus = RequestStatus.TechnicalError;
    public void SetRequestStatusToTimeout() => RequestStatus = RequestStatus.Timeout;
    public void SetRequestStatusToBussinessFailed() => RequestStatus = RequestStatus.BussinessFailed;
}
