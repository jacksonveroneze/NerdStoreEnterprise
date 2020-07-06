using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSE.Mail.API.Extensions;

namespace NSE.Mail.API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
            => _emailSettings = emailSettings.Value;

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSSL
            };

            return client.SendMailAsync(
                new MailMessage("jackson@jacksonveroneze.com", email, subject, htmlMessage) {IsBodyHtml = true}
            );
        }
    }
}
