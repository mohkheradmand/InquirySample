namespace InquirySample.Domain.Entities;

// هنگام اجرا دیتا تستی اضافه میشه برای همین rich Domain پیاده نشده
public sealed class Product
{
    public int Id { get; set; }
    public required string HsCode { get; set; }

}
