using InquirySample.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquirySample.Infrastruture.Persistance.Configurations
{
    public class ProviderAttemptConfiguration : IEntityTypeConfiguration<ProviderAttempt>
    {
        public void Configure(EntityTypeBuilder<ProviderAttempt> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.RequestStatus)
                .IsRequired();

            builder.HasOne(x => x.SharedInquiry)
                .WithMany()
                .HasForeignKey(x => x.SharedInquiryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Inquiry)
                .WithMany()
                .HasForeignKey(x => x.InquiryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
