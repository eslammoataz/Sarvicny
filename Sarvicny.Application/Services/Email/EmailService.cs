using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Sarvicny.Domain.Entities.Emails;


namespace Sarvicny.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;
        private readonly ILogger<EmailService> logger;

        public EmailService(IConfiguration _config, ILogger<EmailService> logger)
        {
            config = _config;
            this.logger = logger;
        }


        public void SendEmail(EmailDto message)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(config["EmailConfiguration:From"]));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Body
            };

            using var smtp = new SmtpClient();

            // Connect to the SMTP server
            smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

            // Authenticate after connecting
            smtp.Authenticate(config["EmailConfiguration:Username"], config["EmailConfiguration:Password"]);

            // Send the email
            smtp.Send(email);

            // Disconnect from the server
            smtp.Disconnect(true);
        }

    }
}