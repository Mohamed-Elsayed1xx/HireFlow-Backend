using System.Security.Cryptography;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HireFlow.Infrastructure.Persistence;

public static class Seeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordService passwordService, IConfiguration configuration, ILogger logger)
    {
        await SeedPlansAsync(context);
        await SeedSuperAdminAsync(context, passwordService, configuration, logger);
    }

    private static async Task SeedPlansAsync(AppDbContext context)
    {
        if (await context.Plans.AnyAsync())
            return;

        var plans = new List<Plan>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Starter",
                Price = 29,
                MaxJobs = 5,
                MaxUsers = 3,
                MaxCandidates = 100,
                Features = "[\"Job Posting\", \"Pipeline\", \"Basic Analytics\"]",
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Growth",
                Price = 79,
                MaxJobs = 20,
                MaxUsers = 10,
                MaxCandidates = 500,
                Features = "[\"Job Posting\", \"Pipeline\", \"Advanced Analytics\", \"Team Management\", \"Email Notifications\"]",
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Enterprise",
                Price = 199,
                MaxJobs = -1,
                MaxUsers = -1,
                MaxCandidates = -1,
                Features = "[\"Unlimited Jobs\", \"Unlimited Users\", \"Full Analytics\", \"Audit Logs\", \"Priority Support\"]",
                IsActive = true
            }
        };

        await context.Plans.AddRangeAsync(plans);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(AppDbContext context, IPasswordService passwordService, IConfiguration configuration, ILogger logger)
    {
        if (await context.Users.AnyAsync(u => u.Role == UserRole.SuperAdmin))
            return;

        // Email isn't a secret, so a sane default is fine. The password is a
        // secret and must never live in source control — read it from
        // config (which in production means an environment variable, e.g.
        // SuperAdmin__Password on Railway/Azure/etc., not a committed file).
        // appsettings.Development.json (gitignored) can set a real value for
        // local convenience; appsettings.json (committed) intentionally
        // leaves this blank.
        var email = configuration["SuperAdmin:Email"];
        if (string.IsNullOrWhiteSpace(email))
            email = "admin@hireflow.com";

        var password = configuration["SuperAdmin:Password"];
        var generated = string.IsNullOrWhiteSpace(password);
        if (generated)
            password = GenerateRandomPassword();
        // password is guaranteed non-null past this point: either it came
        // from config (checked above), or GenerateRandomPassword() just set it.

        var superAdmin = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Super",
            LastName = "Admin",
            Email = email,
            PasswordHash = passwordService.Hash(password!), // guaranteed non-null: see comment above
            Role = UserRole.SuperAdmin,
            IsActive = true
        };

        await context.Users.AddAsync(superAdmin);
        await context.SaveChangesAsync();

        if (generated)
        {
            // Logged once, at first-ever startup only (this whole method
            // short-circuits on every later run since a SuperAdmin now
            // exists). Not a security weakness — anyone with console/log
            // access to the freshly-provisioned server already has far
            // more access than this account grants. Sign in and change
            // the password immediately, then this message never appears
            // again.
            logger.LogWarning(
                "No SuperAdmin:Password configured — generated one for first login. " +
                "Email: {Email} | Password: {Password} " +
                "Set this via an environment variable (SuperAdmin__Password) and change " +
                "the password after your first login; this message will not repeat.",
                email, password);
        }
    }

    private static string GenerateRandomPassword()
    {
        // 24 random bytes -> 32-char base64, then swap the URL-unsafe chars
        // for symbols so it still satisfies a typical "needs a symbol" policy.
        var bytes = RandomNumberGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes).Replace('+', '!').Replace('/', '#');
    }
}