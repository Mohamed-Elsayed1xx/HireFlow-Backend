using System.Security.Claims;
using HireFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    // Backstop for multi-tenant isolation. Every handler already checks
    // `entity.CompanyId != _currentUser.CompanyId` manually (see JobsController,
    // InterviewsController, etc.) — that's correct and stays. This filter is a
    // *second, independent* layer: if a future handler is ever added without
    // that manual check, a cross-tenant row still can't be read, because the
    // query itself never returns it (defense in depth, not a replacement).
    //
    // _bypassTenantFilter is true when there's no reason to restrict by
    // company:
    //   - No HTTP context at all (migrations, `Seeder.SeedAsync` at startup)
    //   - Anonymous requests (e.g. the public job board — those intentionally
    //     span every company's postings)
    //   - SuperAdmin (the admin portal manages every company by design)
    //   - Any authenticated user with no "companyId" claim at all — this is
    //     candidates. Candidates aren't scoped to a company, so there's no
    //     value to compare against; treating a missing claim as "restrict to
    //     null" (the original bug here) silently matched zero rows for every
    //     candidate, including on the public job board once they logged in.
    //     Candidate-specific data access (their own applications, profile,
    //     CV) is already scoped by CandidateId in its own handlers/queries,
    //     independent of this company filter, so bypassing it for candidates
    //     doesn't open up anything they weren't already allowed to see.
    private readonly bool _bypassTenantFilter;
    private readonly Guid? _currentCompanyId;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        var user = httpContextAccessor?.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            _bypassTenantFilter = true;
            return;
        }

        var role = user.FindFirstValue(ClaimTypes.Role);
        if (role == "SuperAdmin")
        {
            _bypassTenantFilter = true;
            return;
        }

        var companyIdClaim = user.FindFirstValue("companyId");
        if (string.IsNullOrEmpty(companyIdClaim))
        {
            _bypassTenantFilter = true;
            return;
        }

        _currentCompanyId = Guid.Parse(companyIdClaim);
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobAssignee> JobAssignees => Set<JobAssignee>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<ApplicationStageHistory> ApplicationStageHistories => Set<ApplicationStageHistory>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<InterviewInterviewer> InterviewInterviewers => Set<InterviewInterviewer>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();
    public DbSet<HiringStage> HiringStages => Set<HiringStage>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Only entities that carry CompanyId directly. Related entities
        // (JobApplication, Interview, Evaluation, ...) are reached through
        // Job/HiringStage in every existing query path, so filtering Job
        // already protects them without the risk of a navigation-based
        // filter breaking existing .Include(...) queries.
        modelBuilder.Entity<Job>()
            .HasQueryFilter(j => _bypassTenantFilter || j.CompanyId == _currentCompanyId);

        modelBuilder.Entity<HiringStage>()
            .HasQueryFilter(h => _bypassTenantFilter || h.CompanyId == _currentCompanyId);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => _bypassTenantFilter || u.CompanyId == null || u.CompanyId == _currentCompanyId);

        base.OnModelCreating(modelBuilder);
    }
}