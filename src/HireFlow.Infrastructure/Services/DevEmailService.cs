using HireFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace HireFlow.Infrastructure.Services;

/// <summary>
/// Development-only email service — logs emails to console instead of sending via SMTP.
/// Prevents failures caused by empty SMTP credentials in local dev environments.
/// Switch to the real EmailService in production via ServiceExtensions.cs conditional registration.
/// </summary>
public class DevEmailService : IEmailService
{
    private readonly ILogger<DevEmailService> _logger;

    public DevEmailService(ILogger<DevEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation(
            "[DEV EMAIL] ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
            "  To:      {To}\n" +
            "  Subject: {Subject}\n" +
            "  Body:    {Body}\n" +
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
            to, subject, body);

        return Task.CompletedTask;
    }
}
