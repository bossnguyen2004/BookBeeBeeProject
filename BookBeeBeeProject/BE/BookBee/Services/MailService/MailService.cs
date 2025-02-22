using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace BookBee.Services.MailService
{
    public class MailService : IMailService
    {

        private readonly string _apiKey;
        private readonly string _from;
        private readonly string _fromName;

        public MailService(IConfiguration config)
        {
            _apiKey = config["SendGridSettings:ApiKey"] ?? throw new Exception("SendGrid API key not found");
            _fromName = config["SendGridSettings:FromName"] ?? throw new Exception("SendGrid username not found");
            _from = config["SendGridSettings:From"] ?? throw new Exception("SendGrid from address not found");
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var client = new SendGridClient(_apiKey);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_from, _fromName),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body
            };
            msg.AddTo(new EmailAddress(email));
            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }
    }
}
