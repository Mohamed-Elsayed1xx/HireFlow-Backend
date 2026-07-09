using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class HiringStageConfiguration : IEntityTypeConfiguration<HiringStage>
{
    public void Configure(EntityTypeBuilder<HiringStage> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Color)
            .HasMaxLength(7);

        builder.HasOne(h => h.Company)
            .WithMany(c => c.HiringStages)
            .HasForeignKey(h => h.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}