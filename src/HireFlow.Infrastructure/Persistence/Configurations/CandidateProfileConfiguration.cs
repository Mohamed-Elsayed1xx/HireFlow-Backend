using System.Text.Json;
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

        // The real column type in every environment that's actually been
        // exercised (Railway, and apparently local too) is jsonb, not
        // text[]. A migration meant to convert it to text[]
        // (FixSkillsColumnType) never actually took effect — it's missing
        // its .Designer.cs pairing, so EF Core's migration runner never
        // recognized or applied it. Rather than untangle that migration
        // history, this maps the model to match the column that's
        // actually there: jsonb, via a value converter that (de)serializes
        // List<string> as a JSON array string. This is what made
        // DemoDataSeeder's Skills writes throw
        // "column Skills is of type jsonb but expression is of type text[]"
        // in production — the model was asserting a type the database
        // didn't have.
        builder.Property(cp => cp.Skills)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        builder.HasOne(cp => cp.Candidate)
            .WithOne(c => c.Profile)
            .HasForeignKey<CandidateProfile>(cp => cp.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
