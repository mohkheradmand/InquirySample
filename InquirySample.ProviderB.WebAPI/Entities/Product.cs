namespace InquirySample.ProviderB.WebAPI.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string HSCode { get; set; }
        public bool IsLicenseValid { get; set; }

    }
}
