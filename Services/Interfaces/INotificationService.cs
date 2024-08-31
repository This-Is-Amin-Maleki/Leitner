using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using SharedLeit;
using System.Net.Mail;

namespace ServicesLeit.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(MailMessage message);
        Task SendEmailAsync(string recipients, string subject, string body, string from);
        Task SendSMSAsync();
        Task SendSMSAsync(string recipients, string? subject, string? body, string? from);
    }
}
