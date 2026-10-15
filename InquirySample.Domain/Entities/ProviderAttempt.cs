using InquirySample.BuildingBlocks.Enums;
using InquirySample.Domain.Enums;
using InquirySample.Domain.Exceptions;

namespace InquirySample.Domain.Entities;

public sealed class ProviderAttempt
{
    public int Id { get; private set; }
    public int InquiryId { get; private set; }
    public Inquiry Inquiry { get; private set; } = null!;
    public int? SharedInquiryId { get; private set; }
    public Inquiry? SharedInquiry { get; private set; }
    public int ProviderId { get; private set; }
    public Provider Provider { get; private set; } = null!;
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



    #region Private Methods
    private void SetInquiryId(int inquiryId)
    {
        if (inquiryId <= 0)
            throw new DomainException("شناسه استعلام نباید 0 یا کوچکتر باشد");

        InquiryId = inquiryId;
    }

    private void SetProviderId(int providerId)
    {
        if (providerId <= 0)
            throw new DomainException("شناسه سرویس دهنده نباید 0 یا کوچکتر باشد");

       ProviderId = providerId;
    }
    #endregion


    #region Public Methods

    public void MarkAsSuccess(DateTime completedAt)
    {
        if (RequestStatus != RequestStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        CompletedAt = completedAt;
        RequestStatus = RequestStatus.Success;
    }

    public void MarkAsTechnicalError(DateTime completedAt)
    {
        if (RequestStatus != RequestStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        CompletedAt = completedAt;
        RequestStatus = RequestStatus.TechnicalError;
    }

    public void MarkAsTimeout(DateTime completedAt)
    {
        if (RequestStatus != RequestStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        CompletedAt = completedAt;
        RequestStatus = RequestStatus.Timeout;
    }

    public void MarkAsBussinessFailed(DateTime completedAt)
    {
        if (RequestStatus != RequestStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        CompletedAt = completedAt;
        RequestStatus = RequestStatus.BussinessFailed;
    }

    #endregion

}
