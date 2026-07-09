using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobAssigneeConfiguration : IEntityTypeConfiguration<JobAssignee>
{
    public void Configure(EntityTypeBuilder<JobAssignee> builder)
    {
        builder.HasKey(ja => ja.Id);

        builder.HasIndex(ja => new { ja.JobId, ja.UserId })
            .IsUnique();

        builder.Property(ja => ja.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(ja => ja.Job)
            .WithMany(j => j.Assignees)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ja => ja.User)
            .WithMany()
            .HasForeignKey(ja => ja.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}