using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.JobId, a.CandidateId })
            .IsUnique();

        builder.Property(a => a.Stage)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Source)
            .HasMaxLength(100);

        builder.HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Candidate)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.RejectedBy)
            .WithMany()
            .HasForeignKey(a => a.RejectedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}