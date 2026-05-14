using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace webStudioBlazor.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpServer)
            || string.IsNullOrWhiteSpace(_settings.SenderEmail)
            || string.IsNullOrWhiteSpace(_settings.Username)
            || string.IsNullOrWhiteSpace(_settings.Password))
        {
            _logger.LogWarning("Email не надіслано: у EmailSettings не заповнені SmtpServer, SenderEmail, Username або Password.");
            return;
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password).ConfigureAwait(false);
        await smtp.SendAsync(email).ConfigureAwait(false);
        await smtp.DisconnectAsync(true).ConfigureAwait(false);

        _logger.LogInformation("Email «{Subject}» надіслано на {To}.", subject, toEmail);
    }
}
