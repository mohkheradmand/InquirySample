namespace InquirySample.Domain.Entities;


/// <summary>
/// هنگام اجرا دیتا تستی اضافه میشه برای همین rich Domain پیاده نشده
/// </summary>
public sealed class Provider
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string BaseUrl { get; set; }
    public int Priority { get; set; }
    public int TimeoutMs { get; set; }
    public bool IsEnabled { get; set; }
}
