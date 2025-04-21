using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace Barbearia.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Barbearia Drummond", _configuration["EmailSettings:From"]));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Bcc.Add(MailboxAddress.Parse("seuemail@exemplo.com")); // <- adiciona você como cópia oculta
        emailMessage.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        emailMessage.Body = builder.ToMessageBody();
        emailMessage.Body.ContentType.Charset = "utf-8";

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]);
        await smtp.SendAsync(emailMessage);
        await smtp.DisconnectAsync(true);
    }
}
