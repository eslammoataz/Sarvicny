using Sarvicny.Domain.Entities.Emails;

namespace Sarvicny.Application.Services.Email
{
    public interface IEmailService
    {
        void SendEmail(EmailDto message);
    }
}