using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.HasKey(cp => cp.Id);

        builder.HasIndex(cp => cp.CandidateId)
            .IsUnique();

        builder.Property(cp => cp.CvUrl)
            .HasMaxLength(500);

        // Store skills as a PostgreSQL text array for clean querying and proper typing
        builder.Property(cp => cp.Skills)
            .HasColumnType("text[]");

        builder.HasOne(cp => cp.Candidate)
            .WithOne(c => c.Profile)
            .HasForeignKey<CandidateProfile>(cp => cp.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
