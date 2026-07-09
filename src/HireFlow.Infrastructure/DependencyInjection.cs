using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using HireFlow.Infrastructure.Persistence;
using HireFlow.Infrastructure.Persistence.Repositories;
using HireFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();
        services.AddHttpClient(); // needed by GoogleAuthService to fetch Google's JWKS

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITotpService, TotpService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        // In DEBUG (local dev): log emails to console — avoids failures from empty SMTP credentials.
        // In Release (production): use the real SMTP EmailService.
#if DEBUG
        services.AddScoped<IEmailService, DevEmailService>();
#else
        services.AddScoped<IEmailService, EmailService>();
#endif
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
}