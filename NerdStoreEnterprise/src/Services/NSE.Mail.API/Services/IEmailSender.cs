using System.Threading.Tasks;

namespace NSE.Mail.API.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
