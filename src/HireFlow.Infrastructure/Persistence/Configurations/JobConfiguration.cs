using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(j => j.Department)
            .HasMaxLength(100);

        builder.Property(j => j.Location)
            .HasMaxLength(150);

        builder.Property(j => j.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(j => j.ExperienceLevel)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(j => j.SalaryMin)
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.SalaryMax)
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(j => j.Company)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.CreatedBy)
            .WithMany()
            .HasForeignKey(j => j.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}