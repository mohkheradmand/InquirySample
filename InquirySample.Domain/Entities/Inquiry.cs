using InquirySample.Domain.Enums;

namespace InquirySample.Domain.Entities
{
    public class Inquiry
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompeletedAt { get; private set; }
        public string? Output { get; private set; }
        public int? ProviderId { get; private set; }
        public Provider? Provider { get; private set; }
        public InquiryStatus InquiryStatus { get; private set; }

        private Inquiry()
        {
        }

        private Inquiry(int productId, DateTime createdAt)
        {
            InquiryStatus = InquiryStatus.Pending;
        }




        public void InquiryStatusToSuccess() => InquiryStatus = InquiryStatus.Success;
        public void InquiryStatusToFailed() => InquiryStatus = InquiryStatus.Failed;
    }
}
