namespace webStudioBlazor.Services;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SmtpServer { get; set; } = "";
    public int Port { get; set; } = 587;
    public string SenderName { get; set; } = "";
    public string SenderEmail { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
