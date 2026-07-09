using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
{
    public void Configure(EntityTypeBuilder<Evaluation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.InterviewId, e.EvaluatorId })
            .IsUnique();

        builder.Property(e => e.Rating)
            .IsRequired();

        builder.Property(e => e.Recommendation)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(e => e.Interview)
            .WithMany(i => i.Evaluations)
            .HasForeignKey(e => e.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Evaluator)
            .WithMany()
            .HasForeignKey(e => e.EvaluatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}