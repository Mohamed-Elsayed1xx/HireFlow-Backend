using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class InterviewInterviewerConfiguration : IEntityTypeConfiguration<InterviewInterviewer>
{
    public void Configure(EntityTypeBuilder<InterviewInterviewer> builder)
    {
        builder.HasKey(ii => ii.Id);

        builder.HasIndex(ii => new { ii.InterviewId, ii.UserId })
            .IsUnique();

        builder.HasOne(ii => ii.Interview)
            .WithMany(i => i.Interviewers)
            .HasForeignKey(ii => ii.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.User)
            .WithMany()
            .HasForeignKey(ii => ii.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}