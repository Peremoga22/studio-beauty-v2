using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using webStudioBlazor.Data;

namespace webStudioBlazor.Services;

public sealed class EmailSmtpOptions
{
    public const string SectionName = "Email:Smtp";

    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "SHINE Cosmetology";
}

/// <summary>Надсилання листів підтвердження та скидання пароля через SMTP.</summary>
public sealed class SmtpEmailSender : IEmailSender<ApplicationUser>
{
    private readonly EmailSmtpOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendHtmlAsync(
            email,
            "Підтвердження email — SHINE Cosmetology",
            $"""
            <p>Вітаємо!</p>
            <p>Підтвердіть електронну пошту для облікового запису на сайті SHINE Cosmetology.</p>
            <p><a href="{confirmationLink}">Підтвердити email</a></p>
            <p>Якщо ви не реєструвалися на сайті, проігноруйте цей лист.</p>
            """);

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendHtmlAsync(
            email,
            "Відновлення пароля — SHINE Cosmetology",
            $"""
            <p>Вітаємо!</p>
            <p>Ви надіслали запит на відновлення пароля на сайті SHINE Cosmetology.</p>
            <p>Щоб створити новий пароль, перейдіть за посиланням нижче:</p>
            <p><a href="{resetLink}">Відновити пароль</a></p>
            <p>Якщо ви не надсилали цей запит, просто проігноруйте цей лист.</p>
            """);

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendHtmlAsync(
            email,
            "Код відновлення пароля — SHINE Cosmetology",
            $"""
            <p>Вітаємо!</p>
            <p>Ваш код для скидання пароля: <strong>{resetCode}</strong></p>
            <p>Якщо ви не надсилали цей запит, проігноруйте цей лист.</p>
            """);

    private async Task SendHtmlAsync(string to, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            _logger.LogWarning("Email SMTP Host is not configured; message to {To} was not sent.", to);
            return;
        }

        var from = string.IsNullOrWhiteSpace(_options.FromEmail) ? _options.User : _options.FromEmail;
        if (string.IsNullOrWhiteSpace(from))
        {
            _logger.LogWarning("Email From address is not configured; message to {To} was not sent.", to);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(from, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(to);

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = string.IsNullOrEmpty(_options.User)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(_options.User, _options.Password),
        };

        await client.SendMailAsync(message).ConfigureAwait(false);
        _logger.LogInformation("Email «{Subject}» sent to {To}.", subject, to);
    }
}
