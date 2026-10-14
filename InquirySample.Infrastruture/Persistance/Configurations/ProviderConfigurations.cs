using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace InquirySample.Infrastruture.Persistance.Configurations;

public class ProviderConfigurations : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x=>x.IsEnabled)
            .HasDefaultValue(true);

        builder.Property(x => x.BaseUrl)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Priority)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.TimeoutMs)
            .IsRequired()
            .HasDefaultValue(2000);
    }
}
