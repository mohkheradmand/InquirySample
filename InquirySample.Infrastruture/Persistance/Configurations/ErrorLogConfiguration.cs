using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InquirySample.Infrastruture.Persistance.Configurations
{
    public class ErrorLogConfiguration : IEntityTypeConfiguration<Domain.Entities.ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Exception)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Source)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
