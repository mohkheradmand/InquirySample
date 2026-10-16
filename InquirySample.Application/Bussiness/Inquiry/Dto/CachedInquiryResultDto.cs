using InquirySample.BuildingBlocks.Enums;

namespace InquirySample.Application.Bussiness.Inquiry.Dto;

public sealed record CachedInquiryResult(
    RequestStatus Status,
    string? Output,
    int? ProviderId,
    int SourceInquiryId);
