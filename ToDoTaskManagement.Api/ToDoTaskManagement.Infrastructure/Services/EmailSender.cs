using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ToDoTaskManagement.Application;
using ToDoTaskManagement.Application.Interfaces;

namespace ToDoTaskManagement.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IOptions<SmtpSettings> options,
            ILogger<EmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mail.To.Add(to);

            try
            {
                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent to {Email}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                //cmented out to prevent exception propagation
                //throw;
            }
        }
    }
}
