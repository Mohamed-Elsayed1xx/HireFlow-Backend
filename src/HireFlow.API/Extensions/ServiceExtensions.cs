using System.Text;
using System.Threading.RateLimiting;
using HireFlow.Application;
using HireFlow.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;

namespace HireFlow.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HireFlow API",
                Version = "v1",
                Description = "Multi-tenant applicant tracking system API."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your access token. Example: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // Fail fast and loudly if Jwt:Secret is missing or too weak, instead
        // of letting an empty string flow straight into
        // `Encoding.UTF8.GetBytes(...)` and quietly sign every token with a
        // zero-length key (appsettings.json ships "Secret": "" as a
        // placeholder — it must be overridden via an environment variable
        // or `dotnet user-secrets` before the app is ever exposed).
        var jwtSecret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Secret is missing or shorter than 32 characters. Set a strong, " +
                "random secret via an environment variable (Jwt__Secret) or " +
                "`dotnet user-secrets set \"Jwt:Secret\" \"<value>\"` — do not leave it " +
                "in appsettings.json.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

        services.AddCors(options =>
        {
            options.AddPolicy("HireFlowCors", policy =>
            {
                policy
    .WithOrigins(
        "http://localhost:4200",
        "http://localhost:4300",
        "http://localhost:4400",
        "https://app.hireflow.com",
        "https://admin.hireflow.com",
        // Was "candidate.hireflow.com" — didn't match the candidate portal's
        // actual configured production domain (projects/candidate/src/environments/environment.prod.ts
        // sets portalDomain: 'careers.hireflow.com'). As written, every API
        // call from the real candidate portal in production would have been
        // silently blocked by CORS — the browser rejects the response before
        // it ever reaches app code, so this would have looked like "the site
        // is broken" with no obvious error pointing here.
        "https://careers.hireflow.com")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // Separate, much stricter policy for login / 2FA / password-reset
            // endpoints. The global 100-req/min-per-IP limiter above is meant
            // for normal API traffic and does nothing to stop credential
            // stuffing or a 2FA code being brute-forced (a 6-digit TOTP code
            // has only 1,000,000 possibilities) — an attacker can send 100
            // guesses/minute from one IP forever under the global limiter
            // alone. This policy caps auth attempts at 5 per 5 minutes,
            // partitioned by IP, applied via [EnableRateLimiting("auth")]
            // on the relevant AuthController actions.
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0
                    }));
        });

        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }
}
