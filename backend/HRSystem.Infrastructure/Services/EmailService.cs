using HRSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HRSystem.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(emailSettings["SenderEmail"]);
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            emailSettings["SmtpHost"], 
            int.Parse(emailSettings["SmtpPort"] ?? "587"), 
            SecureSocketOptions.StartTls);
            
        await smtp.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPassword"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
