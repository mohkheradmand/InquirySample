namespace InquirySample.Domain.Entities;


/// <summary>
/// هنگام اجرا دیتا تستی اضافه میشه برای همین rich Domain پیاده نشده
/// </summary>
public sealed class Product
{
    public int Id { get; set; }
    public required string HsCode { get; set; }

}
