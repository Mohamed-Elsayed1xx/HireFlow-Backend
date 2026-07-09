using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class ApplicationStageHistoryConfiguration : IEntityTypeConfiguration<ApplicationStageHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStageHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.FromStage)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(h => h.ToStage)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(h => h.Application)
            .WithMany(a => a.StageHistory)
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.ChangedBy)
            .WithMany()
            .HasForeignKey(h => h.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}