using ServicesLeit.Interfaces;
using System.Net.Mail;
using System.Net;
using ModelsLeit.DTOs.Notification;
using System.Runtime.InteropServices;

namespace ServicesLeit.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string? _emailAddress;

        public NotificationService(SmtpServiceDto smtpService)
        {
            _smtpClient = new()
            {
                Host = smtpService.Host,
                Port = smtpService.Port,
                Credentials = new NetworkCredential(smtpService.Username, smtpService.Password),
                EnableSsl = smtpService.EnableSsl,
            };
            _emailAddress = smtpService.Address ?? smtpService.Username;
        }

        public async Task SendEmailAsync(string recipients, string subject, string body, [Optional] string from)
        {
            if (string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(_emailAddress))
            {
                from = _emailAddress;
            }

            if(string.IsNullOrEmpty(from))
            {
                throw new Exception("From parameter is not defined!");
            }

            MailMessage mail = new MailMessage(from, recipients, subject, body);
            mail.IsBodyHtml = true;

            await _smtpClient.SendMailAsync(mail);
            //await _smtpClient.SendMailAsync(from, recipients, subject, body);
        }

        public async Task SendEmailAsync(MailMessage message)
        {
            await _smtpClient.SendMailAsync(message);
        }

    }

}