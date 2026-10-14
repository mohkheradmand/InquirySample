using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquirySample.Infrastruture.Persistance.Configurations
{
    public sealed class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.HsCode).IsUnique();

            builder.Property(x => x.HsCode)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
