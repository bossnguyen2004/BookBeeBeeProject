namespace BookBee.Services.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(string email, string subject, string body);
    }
}
