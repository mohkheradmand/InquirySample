using InquirySample.Domain.Enums;
using InquirySample.Domain.Exceptions;

namespace InquirySample.Domain.Entities;

public sealed class Inquiry
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Output { get; private set; }
    public int? ProviderId { get; private set; }
    public Provider? Provider { get; private set; }
    public InquiryStatus InquiryStatus { get; private set; }

    private Inquiry()
    {
    }

    private Inquiry(int productId, DateTime createdAt)
    {
        SetProductId(productId);
        
        InquiryStatus = InquiryStatus.Pending;
        CreatedAt = createdAt;
        
    }

    public static Inquiry Create(int productId, DateTime createdAt)
    {
        return new Inquiry(productId, createdAt);
    }

    #region Private Methods
    private void SetProductId(int productId)
    {
        if (productId <= 0)
            throw new DomainException("شناسه کالا نباید 0 یا کوچکتر باشد");

        ProductId = productId;
    }
    private void SetOutput(string output)
    {
        if (string.IsNullOrEmpty(output))
            throw new DomainException("خروجی نباید خالی باشد");

        Output = output;
    }
    #endregion

    #region Public Methods
    public void MarkAsSuccess(string output, DateTime completedAt, int providerId)
    {
        if (InquiryStatus != InquiryStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        if (providerId <= 0)
            throw new DomainException("شناسه سرویس دهنده نباید 0 یا کوچکتر باشد");

        SetOutput(output);
        ProviderId = providerId;
        CompletedAt = completedAt;
        InquiryStatus = InquiryStatus.Success;
    }
    
    public void MarkAsFailed(DateTime completedAt)
    {
        if (InquiryStatus != InquiryStatus.Pending)
            throw new DomainException("وضعیت درخواست معتبر نیست");

        CompletedAt = completedAt;
        InquiryStatus = InquiryStatus.Failed;
    }
    

    
    #endregion
}
