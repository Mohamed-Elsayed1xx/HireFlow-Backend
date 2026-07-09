using System.Net;
using System.Net.Mail;
using HireFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _configuration["Email:Host"]!;
        var port = int.Parse(_configuration["Email:Port"]!);
        var user = _configuration["Email:User"]!;
        var pass = _configuration["Email:Pass"]!;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(user, "HireFlow"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}