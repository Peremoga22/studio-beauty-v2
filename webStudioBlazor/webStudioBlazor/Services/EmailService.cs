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

        try
        {
            using var smtp = new SmtpClient();

            smtp.Timeout = 15000;

            var secure = ResolveSecureSocketOptions(_settings.Port);

            _logger.LogDebug(
                "SMTP: підключення до {Server}:{Port} ({Secure})",
                _settings.SmtpServer,
                _settings.Port,
                secure);

            await smtp.ConnectAsync(
                    _settings.SmtpServer,
                    _settings.Port,
                    secure)
                .ConfigureAwait(false);

            await smtp.AuthenticateAsync(
                    _settings.Username,
                    _settings.Password)
                .ConfigureAwait(false);

            await smtp.SendAsync(email).ConfigureAwait(false);

            await smtp.DisconnectAsync(true).ConfigureAwait(false);

            _logger.LogInformation("Email надіслано на {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка відправки email на {Email}", toEmail);
            throw;
        }
    }

    // Gmail: 587 + StartTls, або 465 + SslOnConnect (вибір за полем Port у конфігурації).
    private static SecureSocketOptions ResolveSecureSocketOptions(int port) =>
        port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
}
