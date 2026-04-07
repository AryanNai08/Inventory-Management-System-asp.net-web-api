using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IEmailService 
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
